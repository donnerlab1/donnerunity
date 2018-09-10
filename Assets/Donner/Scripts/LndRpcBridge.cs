using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using Grpc.Core;
using UnityEngine;
using Lnrpc;
using Google.Protobuf;
using Grpc.Core.Internal;
using System;
using System.Threading;

namespace Donner
{
    public class LndRpcBridge : MonoBehaviour
    {
        public NewTransactionEventHandler OnNewTransaction;
        public InvoiceSettledEventHandler OnInvoiceSettled;
        public GraphTopologyUpdateEventHandler OnGraphTopologyUpdated;

        string Cert;
        string HostName;
        public LndRpcBridge() {

        }

        Grpc.Core.Channel rpcChannel;
        Grpc.Core.Channel walletRpcChannel;
        Lightning.LightningClient lndClient;
        WalletUnlocker.WalletUnlockerClient walletUnlocker;

        AsyncServerStreamingCall<Transaction> _transactionStream;
        AsyncServerStreamingCall<Invoice> _invoiceStream;
        AsyncServerStreamingCall<GraphTopologyUpdate> _graphStream;


        public async Task<string> ConnectToLnd(string host, string cert)
        {
            Debug.Log("connecting to lnd");
            
            HostName = host;
            Cert = cert;
            var channelCreds = new SslCredentials(cert);
            rpcChannel = new Grpc.Core.Channel(host, channelCreds);
        
            lndClient = new Lightning.LightningClient(rpcChannel);
            InvokeRepeating("TryConnecting", 3, 5);
            return "connected";
        }
        public async Task<string> ConnectToLndWithMacaroon(string host, string cert, string macaroon)
        {
            Debug.Log("connecting to lnd");
            HostName = host;
            Cert = cert;
            var macaroonCallCredentials = new MacaroonCallCredentials(macaroon);
            var sslCreds = new SslCredentials(cert);
            var channelCreds = ChannelCredentials.Create(sslCreds, macaroonCallCredentials.credentials);

            rpcChannel = new Grpc.Core.Channel(host, channelCreds);
            lndClient = new Lightning.LightningClient(rpcChannel);
            InvokeRepeating("TryConnecting", 3, 5);

            return "connected";
        }


        public async Task<string[]> GenerateSeed(string aezeedPassphrase = default(string))
        {
            walletRpcChannel = new Grpc.Core.Channel(HostName, new SslCredentials(Cert));
            walletUnlocker = new WalletUnlocker.WalletUnlockerClient(walletRpcChannel);
            await walletRpcChannel.ConnectAsync();
            var genSeedRequest = new GenSeedRequest();
            if (!string.IsNullOrEmpty(aezeedPassphrase))
                genSeedRequest.AezeedPassphrase = ByteString.CopyFromUtf8(aezeedPassphrase);
            var genSeedResponse = await walletUnlocker.GenSeedAsync(genSeedRequest);
            return genSeedResponse.CipherSeedMnemonic.ToArray(); ;
        }


        public async Task<string> UnlockWallet(string walletPassword, string[] mnemonic)
        {
            walletRpcChannel = new Grpc.Core.Channel(HostName, new SslCredentials(Cert));
            walletUnlocker = new WalletUnlocker.WalletUnlockerClient(walletRpcChannel);
            await walletRpcChannel.ConnectAsync();

            var initWalletRequest = new InitWalletRequest();
            initWalletRequest.WalletPassword = ByteString.CopyFromUtf8(walletPassword);
            initWalletRequest.CipherSeedMnemonic.AddRange(mnemonic);
            try
            {
                var initWalletResponse = await walletUnlocker.InitWalletAsync(initWalletRequest);
                walletRpcChannel.ShutdownAsync().Wait();
                

                return "unlocked";
            } catch(RpcException e)
            {
                if(e.Status.Detail == "wallet already exists")
                {
                    var unlockWalletRequest = new UnlockWalletRequest() {WalletPassword = ByteString.CopyFromUtf8(walletPassword) };
                    var unlockWalletResponse = await walletUnlocker.UnlockWalletAsync(unlockWalletRequest);
                    walletRpcChannel.ShutdownAsync().Wait();

 
                    return "unlocked";
                }
                Debug.Log(e);
            }
            walletRpcChannel.ShutdownAsync().Wait();

            return "not unlocked";
        }

        async void TryConnecting()
        {
            Debug.Log(rpcChannel.State);
            if(rpcChannel.State == ChannelState.Ready)
            {
                CancelInvoke("TryConnecting");
                return;
            }
            await rpcChannel.ConnectAsync();
        }

        public async Task<long> WalletBalance()
        {
            var walletBalanceResponse = await lndClient.WalletBalanceAsync(new WalletBalanceRequest());
            return walletBalanceResponse.ConfirmedBalance;

        }

        public async Task<long> ChannelBalance()
        {
            var channelBalanceRequest = new ChannelBalanceRequest();
            var channelBalanceResponse = await lndClient.ChannelBalanceAsync(channelBalanceRequest);
            return channelBalanceResponse.Balance;
  
        }

        public async Task<string> SendCoins(string address, int amount, int targetConf=-1, int satoshiPerByte=-1)
        {
            var sendCoinsRequest = new SendCoinsRequest() { Addr = address, Amount = amount };
            if (targetConf != -1)
                sendCoinsRequest.TargetConf = targetConf;
            if (satoshiPerByte != -1)
                sendCoinsRequest.SatPerByte = satoshiPerByte;

            var sendCoinsResponse = await lndClient.SendCoinsAsync(sendCoinsRequest);
            return sendCoinsResponse.Txid;
        }

        public async void SubscribeTransactions()
        {

            var request = new GetTransactionsRequest();

            try
            {
                using (_transactionStream = lndClient.SubscribeTransactions(request))
                {

                    while (await _transactionStream.ResponseStream.MoveNext())
                    {
                        var e = new NewTransactionEventArgs();
                        e.Transaction = _transactionStream.ResponseStream.Current;
                        OnNewTransaction(this, e);

                    }

                }
            }catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public async Task<TransactionDetails> GetTransactions() {
            var response = await lndClient.GetTransactionsAsync(new GetTransactionsRequest());
            return response;
        }

        public async Task<string> NewWitnessAdress()
        {
            var response = await lndClient.NewWitnessAddressAsync(new NewWitnessAddressRequest());
            return response.Address;
        }

        public async Task<string> SignMessage(string msg)
        {
            var request = new SignMessageRequest() {Msg = ByteString.CopyFromUtf8(msg)};
            var response = await lndClient.SignMessageAsync(request);
            return response.Signature;
        }

        public async Task<bool> VerifyMessage(string msg, string signature)
        {
            var request = new VerifyMessageRequest() { Msg = ByteString.CopyFromUtf8(msg), Signature = signature };
            var response = await lndClient.VerifyMessageAsync(request);
            return response.Valid;
        }

        public async void ConnectPeer(string pubkey, string host)
        {
            var request = new ConnectPeerRequest();
            request.Addr = parsePubToAddr(pubkey, host);

            var response = await lndClient.ConnectPeerAsync(request);
        }

        public async void DisconnectPeer(string pubkey)
        {
            var request = new DisconnectPeerRequest() {PubKey = pubkey };

            var response = await lndClient.DisconnectPeerAsync(request);
        }

        public async Task<Peer[]> ListPeers()
        {
            var request = new ListPeersRequest();
            var response = await lndClient.ListPeersAsync(request);
            return response.Peers.ToArray();
        }

        public async Task<GetInfoResponse> GetInfo()
        {
            return await lndClient.GetInfoAsync(new GetInfoRequest());
        }

        public async Task<PendingChannelsResponse> PendingChannels()
        {
            return await lndClient.PendingChannelsAsync(new PendingChannelsRequest());
        }

        public async Task<Lnrpc.Channel[]> ListChannels()
        {
            var response = await lndClient.ListChannelsAsync(new ListChannelsRequest());
            return response.Channels.ToArray();
        }

        public async Task<Lnrpc.ChannelCloseSummary[]> ListClosedChannels()
        {
            var response = await lndClient.ClosedChannelsAsync(new ClosedChannelsRequest());
            return response.Channels.ToArray();
        }

        public async Task<string> OpenChannel(string pubkey, int fundingAmount, int pushAmount = 0, int targetConf = 1)
        {

            var request = new OpenChannelRequest() { NodePubkeyString = pubkey, LocalFundingAmount = fundingAmount, PushSat = pushAmount, TargetConf = targetConf };
            var response = await lndClient.OpenChannelSyncAsync(request);
            return response.FundingTxidStr;
        }

        public async Task<CloseStatusUpdate> CloseChannel(ChannelPoint channelPoint,bool force = false,int targetConf = 1)
        {
            var request = new CloseChannelRequest() { ChannelPoint = channelPoint, Force = force, TargetConf = targetConf };
            ;
            using (var stream = lndClient.CloseChannel(request))
            {
                
                    while (await stream.ResponseStream.MoveNext())
                    {
                        return stream.ResponseStream.Current;
                    }
                
            }
            return null;
        }
        
        public async Task<SendResponse> SendPayment(string paymentRequest)
        {
            var request = new SendRequest() { PaymentRequest = paymentRequest };
            return await lndClient.SendPaymentSyncAsync(request);
        }
        public async Task<SendResponse> SendPayment(string paymentRequest, int sat)
        {
            var request = new SendRequest() { PaymentRequest = paymentRequest, Amt= sat};
            return await lndClient.SendPaymentSyncAsync(request);
        }

        public async Task<SendResponse> SendToRoute(string paymentHash, Route[] routes)
        {
            var request = new SendToRouteRequest()
            {
                PaymentHashString = paymentHash,

            };
            request.Routes.Add(routes);
            return await lndClient.SendToRouteSyncAsync(request);
        }
        public async Task<string> AddInvoice(int amount, string memo ="")
        {
            var request = new Invoice() { Value = amount, Memo = memo };
            var response = await lndClient.AddInvoiceAsync(request);
            return response.PaymentRequest;
        }

        public async Task<Invoice[]> ListInvoices(bool pendingOnly = false)
        {
            var response = await lndClient.ListInvoicesAsync(new ListInvoiceRequest() { PendingOnly = pendingOnly });
            return response.Invoices.ToArray();
        }


        public async void SubscribeInvoices()
        {
            
            var request = new InvoiceSubscription();
            try
            {
                using (_invoiceStream = lndClient.SubscribeInvoices(request))
                {

                    while (await _invoiceStream.ResponseStream.MoveNext())
                    {
                        var invoice = _invoiceStream.ResponseStream.Current;
                        if (invoice.Settled)
                        {
                            var e = new InvoiceSettledEventArgs();
                            e.Invoice = invoice;
                            OnInvoiceSettled(this, e);
                        }

                    }


                }
            } catch(Exception e)
            {
                Debug.Log(e);
            }
        }

        public async Task<PayReq> DecodePaymentRequest(string payReq)
        {
            return await lndClient.DecodePayReqAsync(new PayReqString() { PayReq = payReq });
        }

        public async Task<Payment[]> ListPayments()
        {
            var response = await lndClient.ListPaymentsAsync(new ListPaymentsRequest());
            return response.Payments.ToArray();
        }

        public async void DeleteAllPayments()
        {
            await lndClient.DeleteAllPaymentsAsync(new DeleteAllPaymentsRequest());
        }

        public async Task<ChannelGraph> DescribeGraph()
        {
            return await lndClient.DescribeGraphAsync(new ChannelGraphRequest());
        }

        public async Task<ChannelEdge> GetChanInfo(uint channelId)
        {
            return await lndClient.GetChanInfoAsync(new ChanInfoRequest() { ChanId = channelId });
        }

        public async Task<NodeInfo> GetNodeInfo(string pubkey)
        {
            return await lndClient.GetNodeInfoAsync(new NodeInfoRequest() { PubKey = pubkey });
        }

        public async Task<Route[]> QueryRoutes(string pubkey, int amount, int maxRoutes = 1)
        {
            var request = new QueryRoutesRequest()
            {
                PubKey = pubkey,
                Amt = amount,
                NumRoutes = maxRoutes
            };
            var response = await lndClient.QueryRoutesAsync(request);
            return response.Routes.ToArray();
        }

        public async Task<NetworkInfo> GetNetworkInfo()
        {
            return await lndClient.GetNetworkInfoAsync(new NetworkInfoRequest());
        }

        public async void StopDaemon()
        {
            await lndClient.StopDaemonAsync(new StopRequest());
        }


        
        public async void SubscribeChannelGraph()
        {
            
            var request = new GraphTopologySubscription();
            try
            {
                using (_graphStream = lndClient.SubscribeChannelGraph(request))
                {

                    while (await _graphStream.ResponseStream.MoveNext())
                    {
                        var e = new GraphTopologyUpdateEventArgs();
                        e.GraphTopologyUpdate = _graphStream.ResponseStream.Current;
                        OnGraphTopologyUpdated(this, e);
                    }


                }
            } catch(Exception e)
            {
                Debug.Log(e);
            }
        }

        public async void SetDebugLevel(bool show, string level)
        {
            var request = new DebugLevelRequest() { Show = show, LevelSpec = level };
            await lndClient.DebugLevelAsync(request);
        }

        public async Task<FeeReportResponse> FeeReport()
        {
            return await lndClient.FeeReportAsync(new FeeReportRequest());
        }

        public async void UpdateChannelPolicy(double feerate, bool global = true, ChannelPoint channelPoint = null)
        {
            var request = new PolicyUpdateRequest()
            {
                Global = global,
                ChanPoint = channelPoint,
                FeeRate = feerate

            };
            await lndClient.UpdateChannelPolicyAsync(request);
        }

        public async Task<ForwardingEvent[]> ForwardingHistory(uint starttime, uint endtime)
        {
            var request = new ForwardingHistoryRequest()
            {
                StartTime = starttime,
                EndTime = endtime
            };
            var response = await lndClient.ForwardingHistoryAsync(request);
            return response.ForwardingEvents.ToArray();
        }


        LightningAddress parsePubToAddr(string pub, string host)
        {
            var address = new LightningAddress();
            address.Pubkey = pub;
            address.Host = host;
            return address;
        }


        public void Shutdown()
        {
            
            if (_invoiceStream != null)
                _invoiceStream.Dispose();
            if (_graphStream != null)
                _graphStream.Dispose();
            if (_transactionStream != null)
                _transactionStream.Dispose();
            rpcChannel.ShutdownAsync().Wait();


        }

        private void OnApplicationQuit()
        {
            Debug.Log("shutting down lnd interface");
            Shutdown();
        }
    }


    public delegate void NewTransactionEventHandler(object sender, NewTransactionEventArgs e);

    public struct NewTransactionEventArgs
    {
        public Transaction Transaction;
    }

    public delegate void InvoiceSettledEventHandler(object sender, InvoiceSettledEventArgs e);

    public struct InvoiceSettledEventArgs
    {
        public Invoice Invoice;
        
    }

    public delegate void GraphTopologyUpdateEventHandler(object sender, GraphTopologyUpdateEventArgs e);
    
    public struct GraphTopologyUpdateEventArgs
    {
        public GraphTopologyUpdate GraphTopologyUpdate;
    }

    

    
}
