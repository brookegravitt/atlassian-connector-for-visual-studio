﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Atlassian.plvs.api;
using Atlassian.plvs.store;

namespace Atlassian.plvs.models {
    public abstract class AbstractServerModel<T> where T : Server {
        
        private const string SERVER_COUNT = "serverCount";
        private const string SERVER_GUID = "serverGuid_";
        private const string SERVER_NAME = "serverName_";
        private const string SERVER_URL = "serverUrl_";
        private const string SERVER_TYPE = "serverType_";

        public class ModelException : Exception {
            public ModelException(string message) : base(message) {}
        }

        private readonly SortedDictionary<Guid, T> serverMap = new SortedDictionary<Guid, T>();

        protected abstract ParameterStoreManager.StoreType StoreType { get; }
        protected abstract Guid SupportedServerType { get; }
        protected abstract void loadCustomServerParameters(ParameterStore store, T server);
        protected abstract void saveCustomServerParameters(ParameterStore store, T server);
        protected abstract T createServer(Guid guid, string name, string url, string userName, string password);

        public ICollection<T> getAllServers() {
            lock (serverMap) {
                return serverMap.Values;
            }
        }

        public void load() {
            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(StoreType);
            int count = store.loadParameter(SERVER_COUNT, -1);
            if (count != -1) {
                try {
                    for (int i = 1; i <= count; ++i) {
                        string guidStr = store.loadParameter(SERVER_GUID + i, null);
                        Guid guid = new Guid(guidStr);
                        string type = store.loadParameter(SERVER_TYPE + guidStr, null);
                        if (String.IsNullOrEmpty(type) || !SupportedServerType.Equals(new Guid(type))) {
                            // hmm. Is it right? Maybe throw an exception?
                            continue;
                        }

                        string sName = store.loadParameter(SERVER_NAME + guidStr, null);
                        string url = store.loadParameter(SERVER_URL + guidStr, null);

                        T server = createServer(guid, sName, url, null, null);

                        server.UserName = CredentialsVault.Instance.getUserName(server);
                        server.Password = CredentialsVault.Instance.getPassword(server);

                        loadCustomServerParameters(store, server);

                        addServer(server);
                    }
                }
                catch (Exception e) {
                    Debug.WriteLine(e);
                }
            }
        }

        public void save() {
            try {
                ParameterStore store = ParameterStoreManager.Instance.getStoreFor(StoreType);
                store.storeParameter(SERVER_COUNT, serverMap.Values.Count);

                int i = 1;
                foreach (T s in getAllServers()) {
                    string var = SERVER_GUID + i;
                    store.storeParameter(var, s.GUID.ToString());
                    var = SERVER_NAME + s.GUID;
                    store.storeParameter(var, s.Name);
                    var = SERVER_URL + s.GUID;
                    store.storeParameter(var, s.Url);
                    var = SERVER_TYPE + s.GUID;
                    store.storeParameter(var, s.Type.ToString());

                    saveCustomServerParameters(store, s);

                    CredentialsVault.Instance.saveCredentials(s);
                    ++i;
                }
            }
            catch (Exception e) {
                Debug.WriteLine(e);
            }
        }

        public void addServer(T server) {
            lock (serverMap) {
                if (serverMap.ContainsKey(server.GUID)) {
                    throw new ModelException("Server exists");
                }
                serverMap.Add(server.GUID, server);
                save();
            }
        }

        public T getServer(Guid guid) {
            lock (serverMap) {
                return serverMap.ContainsKey(guid) ? serverMap[guid] : null;
            }
        }

        public void removeServer(Guid guid) {
            T s = getServer(guid);
            if (s == null) return;
            removeServer(guid, false);
            CredentialsVault.Instance.deleteCredentials(s);
        }

        public void removeServer(Guid guid, bool nothrow) {
            lock (serverMap) {
                if (serverMap.ContainsKey(guid)) {
                    serverMap.Remove(guid);
                    save();
                }
                else if (!nothrow) {
                    throw new ModelException("No such server");
                }
            }
        }

        public void clear() {
            lock (serverMap) {
                serverMap.Clear();
            }
        }
    }
}