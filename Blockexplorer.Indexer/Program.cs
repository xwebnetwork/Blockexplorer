using System;
using System.Threading;
using System.Threading.Tasks;
using Blockexplorer.BlockProvider.Rpc.Client;
using Blockexplorer.Entities;
using Blockexplorer.BlockProvider.Rpc;
using System.Diagnostics;
using System.Linq;
using Blockexplorer.Core.Domain;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Blockexplorer.Indexer
{
    class Program
    {
        const string CRLF = "\r\n";

        static readonly CancellationTokenSource Cts = new CancellationTokenSource();
        static readonly RpcSettings _rpcSettings;
        static readonly TransactionAdapter _txAdapter;

        static int _currentBlockHeight = 0;
        static int _currentBlockNumber = 1;

        static Program()
        {
            _rpcSettings = new RpcSettings { User = "me", Password = "123", Url = "http://me:123@127.0.0.1:8332/" };
            var options = new UserOptions<RpcSettings>() { Value = _rpcSettings };
            _txAdapter = new TransactionAdapter(options);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Indexer Starting...press any key to cancel.");
            Task.Run(() => Run(Cts.Token));
            Console.ReadLine();
            Console.WriteLine("Cancelling...");
            Cts.Cancel();
        }

        static void Run(CancellationToken token)
        {
            bool shouldWait = false;

            while (!token.IsCancellationRequested)
            {
                if (shouldWait)
                {
                    Console.WriteLine("Going to sleep for 30 seconds...");
                    Thread.Sleep(30000);
                    shouldWait = false;
                }

                try
                {

                    using (var context = new ObsidianChainContext())
                    {
                        using (IDbContextTransaction dbtx = context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                        {
                            try
                            {
                                if (context.BlockEntities.Any())
                                {
                                    _currentBlockNumber = context.BlockEntities.Max(x => x.Id);
                                    _currentBlockHeight = _currentBlockNumber - 1;
                                    _currentBlockHeight++;
                                    _currentBlockNumber++;
                                }

								// now, currentBlockHeight is set to the next block we are looking for to index, however...
								// ...we should ensure we are not indexing the tip.

								var tipHash = _txAdapter.RpcClient.GetBestBlockHash().GetAwaiter().GetResult();
								var tip = _txAdapter.RpcClient.GetBlockAsync(tipHash).GetAwaiter().GetResult();
								if(_currentBlockHeight > tip.Height -3)
								{
									Console.WriteLine($"Block at height {_currentBlockHeight} is not yet mature enough....");
									shouldWait = true;
									continue;
								}

                                string blockHash = _txAdapter.RpcClient.GetBlockHashAsync(_currentBlockHeight).GetAwaiter().GetResult();

                                var result = IndexBlock(context, blockHash);
                                if (result != 0)
                                    return;

                                var stats =context.StatEntities.Find("1"); // insert this row manually
                                stats.BestAdrIndexHeight = _currentBlockHeight;
                                stats.ModifiedDate = DateTime.UtcNow;

                                context.SaveChanges(); // this is still necessary, even when using Commit()
                                dbtx.Commit();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error within the transaction: {ex.Message}");
                                shouldWait = true;
                            }

                        } // Dispose db transaction

                    } // Dispose db connection
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error creating connection / transaction: {e.Message}");
                    Console.WriteLine("Pausing 30 seconds...");
                    Thread.Sleep(30000);
                    Console.WriteLine("Reconnecting...");
                }
            }
        }

        static int IndexBlock(ObsidianChainContext db, string blockHash)
        {
            Console.WriteLine($"Processing block at height {_currentBlockHeight}: {blockHash}");

            GetBlockRpcModel block = _txAdapter.RpcClient.GetBlockAsync(blockHash).GetAwaiter().GetResult();
            if (block == null)
            {
                Console.WriteLine($"Error - could not retrieve block not at height {_currentBlockHeight}: {blockHash}");
                return -1;
            }

            var blockEntity = new BlockEntity { Id = _currentBlockNumber, Height = _currentBlockHeight, BlockHash = blockHash, BlockData = block.OriginalJson };
            db.BlockEntities.Add(blockEntity);
            if (_currentBlockHeight == 0)
            {
                // for the tx in the genesis block, we can't pull transaction data, so we make an exception here
                var genesisBlockTransaction = new TransactionEntity { BlockEntity = blockEntity, Id = block.Tx[0] };
                db.TransactionEntities.Add(genesisBlockTransaction);
                _currentBlockHeight++;
                _currentBlockNumber++;
                db.SaveChanges();
                return 0;
            }

            // Get all transactions from the adapter, so that we can use the logic there
            string[] transactionIds = block.Tx;
            List<Core.Domain.Transaction> blockTransactions = new List<Core.Domain.Transaction>();
            foreach (var txid in transactionIds)
            {
                var transaction = _txAdapter.GetTransaction(txid).GetAwaiter().GetResult();
                transaction.Block = new Block { Height = _currentBlockHeight };
                blockTransactions.Add(transaction);
            }

            // Queue all for insert to the db
            foreach (var blockTx in blockTransactions)
            {
                var transactionEntity = new TransactionEntity() { Id = blockTx.TransactionId, BlockEntity = blockEntity, TransactionData = blockTx.OriginalJson };
               // db.TransactionEntities.Add(transactionEntity);
            }

            // Process all blockTransactions for accounting
            foreach (var blockTx in blockTransactions)
            {
                ProcessTransaction(blockTx, db);
            }

            db.SaveChanges();
            _currentBlockHeight++;
            _currentBlockNumber++;
            return 0;

        }

        static void ProcessTransaction(Core.Domain.Transaction blockTx, ObsidianChainContext db)
        {
            switch (blockTx.TransactionType)
            {
                case TransactionType.PoW_Reward_Coinbase:
                    ProcessPoWReward(blockTx, db);
                    break;
                case TransactionType.PoS_Reward:
                    ProcessStakingReward(blockTx, db);
                    break;
                case TransactionType.Money:
                    ProcessMoneyTransfer(blockTx, db);
                    break;
                default:
                    throw new IndexOutOfRangeException("Unsupported TransactionType.");
            }
        }
        static void ProcessPoWReward(Core.Domain.Transaction tx, ObsidianChainContext db)
        {
            var vout = tx.TransactionsOut[0];
            var address = vout.Address;
            var amount = vout.Value;
            AddressEntity existing = db.AddressEntities.Find(address);
            if (existing == null)
            {
                var newAddress = new AddressEntity
                {
                    Id = address,
                    Balance = amount,
                    LastModifiedBlockHeight = (int)tx.Block.Height,
                    TxIdBlob = tx.TransactionId + CRLF
                };
                InsertAddressIfShouldBeIndexed(newAddress, db);
                return;
            }
            existing.Balance += amount;
            existing.LastModifiedBlockHeight = (int)tx.Block.Height;
            UpdateTxIdBlog(existing,tx.TransactionId);
        }

        static void UpdateTxIdBlog(AddressEntity existing, string txIdToAppend)
        {
            // the last txid is first in the blob
            var oldTxIds = existing.TxIdBlob.Split(CRLF, StringSplitOptions.RemoveEmptyEntries).ToList();
            // append at pos 0
            oldTxIds.Insert(0, txIdToAppend);
            var max250txids = oldTxIds.Take(250).ToArray();
            var sb = new StringBuilder();
            foreach (var txid in max250txids)
                sb.Append(txid + CRLF);
            existing.TxIdBlob = sb.ToString();
        }

        static void ProcessStakingReward(Core.Domain.Transaction tx, ObsidianChainContext db)
        {
            var vin = tx.TransactionIn[0];
            var inAddress = vin.PrevVOutFetchedAddress;
            var oldBalance = vin.PrevVOutFetchedValue;
            Debug.Assert(inAddress == tx.TransactionsOut[1].Address);
            Debug.Assert(inAddress == tx.TransactionsOut[2].Address);
            var outValue1 = tx.TransactionsOut[1].Value;
            var outValue2 = tx.TransactionsOut[2].Value;
            var change = outValue1 + outValue2 - oldBalance;

            // I assume that only pre-existing addresses get a staking reward!
            AddressEntity existing = db.AddressEntities.Find(inAddress);
            Debug.Assert(existing != null);
            existing.Balance += change;
            existing.LastModifiedBlockHeight = (int)tx.Block.Height;
            UpdateTxIdBlog(existing, tx.TransactionId);
        }


        static void ProcessMoneyTransfer(Core.Domain.Transaction tx, ObsidianChainContext db)
        {
            IList<VIn> vins = tx.TransactionIn;
            List<string> inAdresses = new List<string>();
            foreach (var vin in vins)
            {
                AddressEntity existing = db.AddressEntities.Find(vin.PrevVOutFetchedAddress);

                if (existing == null) // work around broken index till indexing is re-run
                {
                    Console.WriteLine($"This should never happen: {vin.PrevVOutFetchedAddress} could not be found.");
                    continue;
                }
                   

                inAdresses.Add(existing.Id);
                existing.Balance -= vin.PrevVOutFetchedValue;
                existing.LastModifiedBlockHeight = (int)tx.Block.Height;
                UpdateTxIdBlog(existing, tx.TransactionId);
            }
            IList<Out> vouts = tx.TransactionsOut;
            foreach (var vout in vouts)
            {
                string outAdress = vout.Address;
                if (outAdress.StartsWith("OP_RETURN", StringComparison.OrdinalIgnoreCase))
                    outAdress = "OP_RETURN";

                AddressEntity existing = db.AddressEntities.Find(outAdress);
                if (existing != null)
                {
                    existing.Balance += vout.Value;
                    if (!inAdresses.Contains(existing.Id))
                    {
                        UpdateTxIdBlog(existing, tx.TransactionId);
                        existing.LastModifiedBlockHeight = (int)tx.Block.Height;
                    }
                }
                else
                {
                    var newAddress = new AddressEntity
                    {
                        Id = vout.Address,
                        Balance = vout.Value,
                        LastModifiedBlockHeight = (int)tx.Block.Height,
                        TxIdBlob = tx.TransactionId + CRLF
                    };
                    InsertAddressIfShouldBeIndexed(newAddress, db);
                }
            }
        }

        static void InsertAddressIfShouldBeIndexed(AddressEntity address, ObsidianChainContext db)
        {
            if (address.Id.StartsWith("OP_RETURN", StringComparison.OrdinalIgnoreCase))
                address.Id = "OP_RETURN";
            if (address.Id != TransactionAdapter.NonStandardAddress)
                db.AddressEntities.Add(address);
        }
    }
}
