namespace Alpha.CloneAccount.Services
{
    using System;
    using System.Xml;
    using Alpha.CloneAccount.Criteria;
    using Fanex.Data.Repository;
    using Sunplus.Libraries.APILogger;

    public class CasinoAPILogger : IAPILogger
    {
        private readonly IDynamicRepository repository;

        public CasinoAPILogger(IDynamicRepository repository = null)
        {
            this.repository = repository ?? new DynamicRepository();
        }

        public void Write(int actionId, string functionName, string strackingValue, string returnValue, int returnCode)
        {
            var userId = 0;
            var custId = 0;

            try
            {
                var xmlDocument = LoadXmlDocument(strackingValue);
                userId = GetValueByTagNames(xmlDocument, new[] { "SignupBy", "ModifyBy" });
                custId = GetValueByTagNames(xmlDocument, new[] { "intIBCCustID", "IBCCustID" });
            }
            catch (Exception)
            {
                // Todo: Write error
            }

            WriteLog(userId, custId, actionId, functionName, strackingValue, returnValue, returnCode);
        }

        private static XmlDocument LoadXmlDocument(string xml)
        {
            var tracking = new XmlDocument();
            tracking.LoadXml(xml);

            return tracking;
        }

        private static int GetValueByTagNames(XmlDocument xmlDocument, params string[] tagNames)
        {
            var tagNameLength = tagNames.Length;

            for (int i = 0; i < tagNameLength; i++)
            {
                var elements = xmlDocument.GetElementsByTagName(tagNames[i]);

                if (elements.Count != 0)
                {
                    return TryParse(elements.Item(0).InnerText);
                }
            }

            return 0;
        }

        private static int TryParse(string value)
        {
            int intValue;
            int.TryParse(value, out intValue);

            return intValue;
        }

        private void WriteLog(
            int userId,
            int custId,
            int actionId,
            string functionName,
            string strackingValue,
            string returnValue,
            int returnCode)
        {
            try
            {
                repository.Execute(new CasinoAPILogCriteria(
                    userId,
                    custId,
                    actionId,
                    functionName,
                    strackingValue,
                    returnValue,
                    returnCode));
            }
            catch (Exception)
            {
                // Todo: Write error
            }
        }
    }
}
