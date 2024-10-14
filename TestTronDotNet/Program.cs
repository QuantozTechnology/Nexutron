using System;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Nexutron;
using Nexutron.Contracts;
using Nexutron.Extensions;

namespace TestTronDotNet
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello NexuTron World!");


            TestContractTransation();
        }


        public static void GenerateWalletOffline()
        {

            var key = TronECKeyGenerator.GenerateKey();

            var address = key.GetPublicAddress();

            Console.WriteLine(address);

        }

        public static void TestKeyGeneration()
        {
            // Should be TBDCyrZ1hT1PDDFf2yRABwPrFica5qqPUX
            var key = new TronECKey("fd605fb953fcdabb952be161265a75b8a3ce1c0def2c7db72265f9db9a471be4");
            Console.WriteLine(key.GetPublicAddress());
        }


        public static void TestTrxTransation()
        {
            var walletPrivateKey = "62075119d64f17ebd3248df7a864cd84380fcb9e5771f0968af2167a25717bb2";

            var ecKey = new TronECKey(walletPrivateKey);
            var from = ecKey.GetPublicAddress();
            //Receiving wallet
            var to = "TH8fU6BLpU6EjqvZTWWSB1uhpEVxzT35Xj";

            //Pay 2 trx
            var amount = 2 * 1_000_000L;

            IServiceCollection services = new ServiceCollection();
            services.AddTronDotNet(x =>
            {
                x.Channel = new GrpcChannelOption { Host = "grpc.shasta.trongrid.io", Port = 50051 };
                x.SolidityChannel = new GrpcChannelOption { Host = "grpc.shasta.trongrid.io", Port = 50052 };
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
                x.Channel = new GrpcChannelOption { Host = "grpc.shasta.trongrid.io", Port = 50051 };
                x.SolidityChannel = new GrpcChannelOption { Host = "grpc.shasta.trongrid.io", Port = 50052 };
                // x.ApiKey = "07rgc8e4-7as1-4d34-d334-fe40ai6gc542";
                //I thought it was necessary to fill in, but it seems that it can be used without filling in
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
