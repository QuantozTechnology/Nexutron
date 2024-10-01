using Google.Protobuf;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Nexutron;
using Nexutron.Contracts;
using Nexutron.Extensions;
using Nexutron.Helpers;
using Nexutron.Protocol;
using System;

namespace TestTronDotNet
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello NexuTron World!");


            TestTransactionParsing();
        }


        public static void GenerateWalletOffline()
        {

            var key = TronECKeyGenerator.GenerateKey(TronNetwork.MainNet);

            var address = key.GetPublicAddress();

            Console.WriteLine(address);

        }

        public static void TestKeyGeneration()
        {
            // Should be TBDCyrZ1hT1PDDFf2yRABwPrFica5qqPUX
            var key = new TronECKey("fd605fb953fcdabb952be161265a75b8a3ce1c0def2c7db72265f9db9a471be4", TronNetwork.MainNet);
            Console.WriteLine(key.GetPublicAddress());
        }

        public static void TestSigning()
        {
            var walletPrivateKey = "fd605fb953fcdabb952be161265a75b8a3ce1c0def2c7db72265f9db9a471be4";

            var ecKey = new TronECKey(walletPrivateKey, TronNetwork.MainNet);
            var from = ecKey.GetPublicAddress();

            //Receiving wallet
            var to = "TEiMQZpHs4N4HuTKP3xcCKZ68XSQSfEbMW";

            //Play 0.001 trx
            var amount = 1 * 1_000L;

            IServiceCollection services = new ServiceCollection();
            services.AddTronDotNet(x =>
            {
                x.Network = TronNetwork.TestNet;
                x.Channel = new GrpcChannelOption { Host = "3.225.171.164", Port = 50051 };
                x.SolidityChannel = new GrpcChannelOption { Host = "3.225.171.164", Port = 50052 };
                // x.ApiKey = "07rgc8e4-7as1-4d34-d334-fe40ai6gc542";
                //I thought it was necessary to fill in, but it seems that it can be used without filling in
                x.ApiKey = "apikey";
            });

            services.AddLogging();
            var service = services.BuildServiceProvider();
            var transactionClient = service.GetService<ITransactionClient>();

            var transactionExtension = transactionClient.CreateTransactionAsync(from, to, amount).Result;

            var transactionSigned = transactionClient.GetTransactionSign(transactionExtension.Transaction, walletPrivateKey);

            //Get sign
            var signed = JsonConvert.SerializeObject(transactionSigned);
            transactionSigned.CalculateSize();

            Console.WriteLine("-SIGN-");

            Console.WriteLine(signed);
            Console.WriteLine("-TXID-");
            Console.WriteLine(transactionSigned.GetTxid());

            //Send out
            var result = transactionClient.BroadcastTransactionAsync(transactionSigned).Result;
            Console.WriteLine("-RESULT-");
            Console.WriteLine(JsonConvert.SerializeObject(result));

            Console.Write("-DONE-");
        }

        public static void TestTransactionParsing()
        {


            IServiceCollection services = new ServiceCollection();
            services.AddTronDotNet(x =>
            {
                x.Network = TronNetwork.TestNet;
                x.Channel = new GrpcChannelOption { Host = "grpc.shasta.trongrid.io", Port = 50051 };
                x.SolidityChannel = new GrpcChannelOption { Host = "grpc.shasta.trongrid.io", Port = 50052 };
                //x.Channel = new GrpcChannelOption { Host = "3.225.171.164", Port = 50051 };
                //x.SolidityChannel = new GrpcChannelOption { Host = "3.225.171.164", Port = 50052 };
                // x.ApiKey = "07rgc8e4-7as1-4d34-d334-fe40ai6gc542";
                //I thought it was necessary to fill in, but it seems that it can be used without filling in
                x.ApiKey = "apikey";
            });

            services.AddLogging();
            var service = services.BuildServiceProvider();
            var walletClient = service.GetService<IWalletClient>();
            var wallet = walletClient.GetWalletClient();

            var account = new Account();
            account.Address = AccountHelper.ParseAddress("TVE2zEUu17oLhHZ2n26ye6SqP7ZLWJtBHA");

            var testresource = wallet.GetAccountNet(account);

            var testBlock = wallet.GetBlockByNum(new NumberMessage { Num = 45052774 });

            var blockTx = wallet.GetTransactionInfoByBlockNum(new NumberMessage { Num = 45052774 });
            var otherBlockTx = wallet.GetTransactionInfoByBlockNum(new NumberMessage { Num = 46884446 });

            // https://shasta.tronscan.org/#/transaction/17821228a79904c23bd35e566f320c2d43e6940c0d44bc8d70f257f3485459bb
            var trxTx = wallet.GetTransactionById(new BytesMessage { Value = ByteString.FromBase64("KLO1UrtBQyeDtdFiRNE2FAyP2DNtswzorzCGdYAGdGk=") });
            var testTx = wallet.GetTransactionById(new BytesMessage { Value = ByteString.FromBase64("SZ07OILqQ5niylOH+5kMHih8eq9EGSIp5+3CSJOvKCQ=") });
            var contractTx = wallet.GetTransactionById(new BytesMessage { Value = ByteString.FromBase64("/MLD7DLhMWW6oVzDO7Jefzoeqzg5ehi0wPPeDkFPpSw=") });

            Console.Write("-DONE-");
        }


        public static void TestTrxTransation()
        {
            var walletPrivateKey = "a56dc78b73a892f9f94e1d28c51b87fa3e1a08fe6c291b0e083e8eac5aa6a295";

            var ecKey = new TronECKey(walletPrivateKey, TronNetwork.MainNet);
            var from = ecKey.GetPublicAddress();
            //Receiving wallet
            //var to = "TVE2zEUu17oLhHZ2n26ye6SqP7ZLWJtBHA";
            var to = "TEv1PU2g7PfNNUyD4ZDidiSzgaFPDHvjsc";

            //Play 20 trx
            var amount = 20 * 1_000_000L;

            IServiceCollection services = new ServiceCollection();
            services.AddTronDotNet(x =>
            {
                x.Network = TronNetwork.TestNet;
                x.Channel = new GrpcChannelOption { Host = "grpc.shasta.trongrid.io", Port = 50051 };
                x.SolidityChannel = new GrpcChannelOption { Host = "grpc.shasta.trongrid.io", Port = 50052 };
                // x.ApiKey = "07rgc8e4-7as1-4d34-d334-fe40ai6gc542";
                //I thought it was necessary to fill in, but it seems that it can be used without filling in
                x.ApiKey = "apikey";
            });

            services.AddLogging();
            var service = services.BuildServiceProvider();
            var transactionClient = service.GetService<ITransactionClient>();
            var transactionExtension = transactionClient.CreateTransactionAsync(from, to, amount).Result;

            var transactionSigned = transactionClient.GetTransactionSign(transactionExtension.Transaction, walletPrivateKey);

            //Get sign
            var signed = JsonConvert.SerializeObject(transactionSigned);

            Console.WriteLine("-SIGN-");

            Console.WriteLine(signed);
            Console.WriteLine("-TXID-");
            Console.WriteLine(transactionSigned.GetTxid());

            //Send out
            var result = transactionClient.BroadcastTransactionAsync(transactionSigned).Result;
            Console.WriteLine("-RESULT-");
            Console.WriteLine(JsonConvert.SerializeObject(result));
        }


        /// <summary>
        /// Transfer USDC
        /// </summary>
        public static void TestContractTransation()
        {
            //The private key of the transmitter
            var walletPrivateKey = "62075119d64f17ebd3248df7a864cd84380fcb9e5771f0968af2167a25717bb2";

            IServiceCollection services = new ServiceCollection();
            services.AddTronDotNet(x =>
            {
                x.Network = TronNetwork.MainNet;
                x.Channel = new GrpcChannelOption { Host = "3.225.171.164", Port = 50051 };
                x.SolidityChannel = new GrpcChannelOption { Host = "3.225.171.164", Port = 50052 };
                x.ApiKey = "apikey";
            });


            services.AddLogging();

            var service = services.BuildServiceProvider();
            var walletClient = service.GetService<IWalletClient>();

            var account = walletClient.GetAccount(walletPrivateKey);

            //USDT TOKEN
            var contractAddress = "TXLAQ63Xg1NAzckPwKHvzw7CSEmLMEqcdj";

            //Wallet
            var to = "TH8fU6BLpU6EjqvZTWWSB1uhpEVxzT35Xj";

            var amount = 99;
            //Handling fee 10 TRX
            var feeAmount = 10 * 1_000_000L;

            var contractClientFactory = service.GetService<IContractClientFactory>();

            var contractClient = contractClientFactory.CreateClient(ContractProtocol.TRC20);

            //Remarks can only be in English
            var result = contractClient.TransferAsync(contractAddress, account, to, amount, "Miladsoft", feeAmount).Result;

            Console.WriteLine("-- RESULT --");
            Console.WriteLine(JsonConvert.SerializeObject(result));
        }
    }
}
