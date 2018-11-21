namespace Alpha.CloneAccount.Services
{
    using System;
    using System.Collections;
    using System.Data;
    using Params;

    public interface SportbooksCloneMemberSpParamsBuilder
    {
        Hashtable BuildSpParams(DataRow sportbooksSetting, CloneCustomerParam param, string hashPassword);
    }

    public class SportbooksCloneMemberSpParamsBuilderImpl : SportbooksCloneMemberSpParamsBuilder
    {
        public Hashtable BuildSpParams(DataRow sportbooksSetting, CloneCustomerParam param, string hashPassword)
        {
            var spParams = new Hashtable();

            spParams.Add("1", new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "9999" });
            spParams.Add("1_live", new string[] { "1", "3", "7", "8" });
            spParams.Add("2", new string[] { "1", "2", "3", "7", "8", "20", "10", "9999" });
            spParams.Add("3", new string[] { "1", "2", "3", "20", "10" });

            spParams.Add("5", new string[] { "1", "2", "3", "20", "9999", "10" });

            spParams.Add("8", new string[] { "1", "3", "20", "10" });
            spParams.Add("10", new string[] { "1", "2", "3", "20", "10" });
            spParams.Add("11", new string[] { "1", "20", "10" });
            spParams.Add("99", new string[] { "1", "2", "3", "9", "20", "10" });
            spParams.Add("161", new string[] { "81", "83", "86", "88" });
            spParams.Add("161_live", new string[] { "85", "86", "87", "89", "90" });

            AddAccountInfoSpParams(sportbooksSetting, param, hashPassword, spParams);
            AddBettingLimitSpParams(sportbooksSetting, spParams);
            AddPositionTakingSpParams(sportbooksSetting, spParams);

            return spParams;
        }

        private void AddAccountInfoSpParams(DataRow sportbooksSetting, CloneCustomerParam param, string hashPassword, Hashtable spParams)
        {
            spParams.Add("custid", 0);
            spParams.Add("username", param.UserName);
            spParams.Add("userpwd", hashPassword);
            spParams.Add("firstname", param.FirstName);
            spParams.Add("lastname", param.LastName);
            spParams.Add("phone", param.Phone);
            spParams.Add("mobilephone", param.MobilePhone);
            spParams.Add("fax", param.Fax);
            spParams.Add("credit", param.Credit);
            spParams.Add("copyfromcustid", param.SourceCustId);
            spParams.Add("uplineid", param.ActorId);
            spParams.Add("systemid", param.SystemId);
            spParams.Add("playerdiscount", sportbooksSetting["playerdiscount"]);
            spParams.Add("playerdiscountcs", sportbooksSetting["playerdiscountcs"]);
            spParams.Add("playerdiscount1x2", sportbooksSetting["playerdiscount1x2"]);
            spParams.Add("playerdiscountnumber", sportbooksSetting["playerdiscountnumber"]);
            spParams.Add("playerdiscountmyanmarodds", sportbooksSetting["playerdiscountmyanmarodds"]);
            spParams.Add("suspended", sportbooksSetting["suspended"]);
            spParams.Add("recommend", param.ActorId);
            spParams.Add("ugroup", sportbooksSetting["ugroup"]);
            spParams.Add("closed", sportbooksSetting["closed"]);
            spParams.Add("issearchandcopy", 1);
            spParams.Add("disablenumber", sportbooksSetting["disablenumber"]);
            spParams.Add("language", param.Language);
            spParams.Add("ip", param.IP);
            spParams.Add("userid", param.ActorId);
            spParams.Add("creator", param.ActorId);
            spParams.Add("subaccid", param.ActorSubAccId);
            spParams.Add("errcode", 0);
            spParams.Add("errmsg", string.Empty);
        }

        private void AddBettingLimitSpParams(DataRow sportbooksSetting, Hashtable spParams)
        {
            var sportTypes = new[] { "1", "1mp", "2", "3", "5", "8", "10", "11", "99", "99mp", "161" };
            var parameters = new[] { "minbetv{0}", "maxbetv{0}", "maxpermatchv{0}" };

            foreach (string item in sportTypes)
            {
                if (item == "1")
                {
                    spParams.Add("minbet", sportbooksSetting["minbet"]);
                    spParams.Add("maxbet", sportbooksSetting["maxbet"]);
                    spParams.Add("maxpermatch", sportbooksSetting["maxpermatch"]);
                }
                else
                {
                    if (item == "161")
                    {
                        spParams.Add("maxperballv161", sportbooksSetting["maxperballv161"]);
                    }

                    foreach (string param in parameters)
                    {
                        var formatParam = string.Format(param, item);

                        if (!spParams.ContainsKey(formatParam))
                        {
                            spParams.Add(formatParam, sportbooksSetting[formatParam]);
                        }
                    }
                }
            }
        }

        private void AddPositionTakingSpParams(DataRow sportbooksSetting, Hashtable spParams)
        {
            var parameter = string.Empty;
            var parameter_max = string.Empty;

            foreach (string key in spParams.Keys)
            {
                var bettypes = (string[])spParams[key];

                switch (key)
                {
                    case "1":
                        parameter = "apositiontaking_{0}";
                        parameter_max = "maxpos_{0}";
                        break;

                    case "1_live":
                        parameter = "alivepositiontaking_{0}";
                        parameter_max = "maxlpos_{0}";
                        break;

                    case "161_live":
                        parameter = "v161alivepositiontaking_{0}";
                        parameter_max = "maxv161lpos_{0}";
                        break;

                    default:
                        parameter = string.Format("v{0}apositiontaking_", key) + "{0}";
                        parameter_max = string.Format("maxv{0}pos_", key) + "{0}";
                        break;
                }

                foreach (string bettype in bettypes)
                {
                    var param = string.Format(parameter, bettype);
                    var param_max = string.Format(parameter_max, bettype);

                    double min = Math.Min(Convert.ToDouble(sportbooksSetting[param]), Convert.ToDouble(sportbooksSetting[param_max]));
                    spParams.Add(param, min);
                }
            }
        }
    }
}
