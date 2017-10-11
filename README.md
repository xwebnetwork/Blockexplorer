Blockexplorer
=============

Blockexplorer Implementation in C# / [.NET Core](https://dotnet.github.io/)
------------------------------------------------

Recommended IDE is Visual Studio 2017 Community Edition

Running Blockexplorer
---------------------

Configure the RPC provider in appsettings.json. The default connection for development is:

```
"RpcSettings": {
    "Url": "http://me:123@127.0.0.1:8332/",
    "User": "me",
    "Password": "123" 
  } 
 ```

Then, configure your locally running Obsidian-Qt to match the appsettings.json. In obsdian.conf set:

```
server=1
txindex=1
rpcuser=me
rpcpassword=123
printtoconsole=1
rpcport=8332
```

To run:

```
git clone https://github.com/netsfx/Blockexplorer.git  
cd Blockexplorer/Blockexplorer

dotnet restore
dotnet run

```

Obsidian Slack Developer Contacts:

@blackstone (github: netsfx)

@jacobz




License: MIT
