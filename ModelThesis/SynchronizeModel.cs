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

        public Mal.Providers.MalContextParams CreateContextParam()
        {
            var context = new Mal.Providers.MalContextParams()
            {
                OdbServerName = ConnectionToDbCk11,
                OdbInstanseName = NameOfDb,
                OdbModelVersionId = VersionOfModel,
            };

            return context;
        }

        public Mal.Providers.Mal.MalProvider CreateProvider()
        {
            var provider = new Mal.Providers.Mal.MalProvider
                (CreateContextParam(), Mal.Providers.MalContextMode.Open, "malApiPtur");

            return provider;
        }

        public Mal.ModelImage CreateModelImage(Mal.Providers.Mal.MalProvider malProvider)
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

        public List<UuidContainer> UpdateVoltageUuid(string substationUuid)
        {
            var provider = CreateProvider();
            var model = CreateModelImage(provider);

            var result = new List<UuidContainer>();
            var patternMax = @"\w*\s*max\s*\w*";
            var patternMin = @"\w*\s*min\s*\w*";

            if (model != null)
            {
                var substations = model.GetObject(Guid.Parse(substationUuid));
                foreach (Substation substation in substations.GetByAssocM("ChildObjects"))
                {
                    var tempMaxList = new List<string>();
                    var tempMinList = new List<string>();
                    var tempFactList = new List<string>();
                    foreach (var oru in substation.GetByAssocM("ChildObjects"))
                    {
                        foreach (var folder in oru.GetByAssocM("ChildObjects"))
                        {
                            foreach (var analog in folder.GetByAssocM("ChildObjects"))
                            {
                                foreach (RemoteAnalogValue analogValue in analog.GetByAssocM("ChildObjects"))
                                {
                                    if (Regex.IsMatch(analogValue.name.ToString(), patternMax))
                                    {
                                        tempMaxList.Add(analogValue.Uid.ToString());
                                        continue;
                                    }
                                    if (Regex.IsMatch(analogValue.name.ToString(), patternMin))
                                    {
                                        tempMinList.Add(analogValue.Uid.ToString());
                                    }
                                    else
                                    {
                                        tempFactList.Add(analogValue.Uid.ToString());
                                    }
                                }
                            }
                        }
                    }
                    var tempUuid = "93C55F61-4960-485C-88A3-93C240DEBAB9";
                    result.Add(new UuidContainer(substation.name, tempFactList[0], tempMaxList[0], tempMinList[0],
                        tempUuid));
                    
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

        public List<UuidContainer> UpdateCurrentUuid(string currentFolderUuid)
        {
            var provider = CreateProvider();
            var model = CreateModelImage(provider);

            var result = new List<UuidContainer>();
            var patternCurrent = @"\w*\s*ДДТН\s*\w*";

            if (model != null)
            {
                var lineFolder = model.GetObject(Guid.Parse(currentFolderUuid));
                foreach (Line line in lineFolder.GetByAssocM("ChildObjects"))
                {
                    var tempCurrentList = new List<string>();
                    var tempFactList = new List<string>();
                    foreach (Analog analog in line.GetByAssocM("ChildObjects"))
                    {
                        foreach (RemoteAnalogValue analogValue in analog.GetByAssocM("ChildObjects"))
                        {
                            if (Regex.IsMatch(analogValue.name.ToString(), patternCurrent))
                            {
                                tempCurrentList.Add(analogValue.Uid.ToString());
                            }
                            else
                            {
                                tempFactList.Add(analogValue.Uid.ToString());
                            }
                        }
                    }

                    result.Add(new UuidContainer(line.name, tempFactList[0], tempCurrentList[0]));
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

        //public void TestModel(string uuid)
        //{
        //    var provider = CreateProvider();
        //    var model = CreateModelImage(provider);
        //    var substatins = model.GetObject(Guid.Parse(uuid));


        //    Console.WriteLine(substatins.GetByAssocM("ChildObjects")[0].GetByAssocM("ChildObjects")[0].GetType());
        //}
    }
}
