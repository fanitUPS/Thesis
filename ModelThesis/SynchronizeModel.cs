using Monitel.Mal;
using Monitel.Mal.Context.CIM16;
using Monitel.Rtdb.Api.Config;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using System.Text.RegularExpressions;
using Mal = Monitel.Mal;

namespace ModelThesis
{
    public class SynchronizeModel
    {
        public string ConnectionToDbCk11 { get; private set; }

        public string NameOfDb { get; private set; }

        public int VersionOfModel { get; private set; }

        public SynchronizeModel(string connectionCk11, string nameOfDb, int versionOfmodel)
        {
            ConnectionToDbCk11 = connectionCk11;
            NameOfDb = nameOfDb;
            VersionOfModel = versionOfmodel;
        }

        private Mal.Providers.MalContextParams CreateContextParam()
        {
            var context = new Mal.Providers.MalContextParams()
            {
                OdbServerName = ConnectionToDbCk11,
                OdbInstanseName = NameOfDb,
                OdbModelVersionId = VersionOfModel,
            };

            return context;
        }

        private Mal.Providers.Mal.MalProvider CreateProvider()
        {
            var provider = new Mal.Providers.Mal.MalProvider
                (CreateContextParam(), Mal.Providers.MalContextMode.Open, "malApiPtur");

            return provider;
        }

        private Mal.ModelImage CreateModelImage(Mal.Providers.Mal.MalProvider malProvider)
        {
            return new ModelImage(malProvider);
        }

        public List<UuidContainer> UpdatePowerUuid(string branchGroupFolderUuid)
        {
            var provider = CreateProvider();
            var model = CreateModelImage(provider);

            var result = new List<UuidContainer>();
            var patternMdp = @"\w*\s*МДП\s*\w*";

            if (model != null)
            {
                var branchGroupFolder = model.GetObject(Guid.Parse(branchGroupFolderUuid));
                foreach (BranchGroup branchGroup in branchGroupFolder.GetByAssocM("ChildObjects"))
                {
                    var tempMdpList = new List<string>();
                    var tempFactList = new List<string>();
                    foreach (Analog analog in branchGroup.GetByAssocM("ChildObjects"))
                    {
                        foreach (RemoteAnalogValue analogValue in analog.GetByAssocM("ChildObjects"))
                        {
                            if (Regex.IsMatch(analog.name.ToString(), patternMdp))
                            {
                                tempMdpList.Add(analogValue.Uid.ToString());
                            }
                            else
                            {
                                tempFactList.Add(analogValue.Uid.ToString());
                            }
                        }
                    }
                    result.Add(new UuidContainer(branchGroup.name, tempFactList[0], tempMdpList[0]));
                }
                provider.Dispose();

                return result;
            }
            else
            {
                provider.Dispose();
                throw new ArgumentException
                    ($"Не удалось подключиться к модели версии {this.VersionOfModel}");
            }
        }
    }
}
