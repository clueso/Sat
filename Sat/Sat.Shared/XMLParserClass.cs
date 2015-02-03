using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sat
{
    class XMLParserClass
    {
        private XElement XMLDocElements;
        
        public XMLParserClass(string XMLDocumentName)
        {
            try
            {
                XMLDocElements = XElement.Load(XMLDocumentName);
            }
            catch(Exception e)
            {
                
            }
            
        }

        public void ReadCitiesInProvince(string ProvinceName, List<string> CityNameList)
        {
            if(XMLDocElements != null)
            {
                IEnumerable<XElement> ProvinceNode =
                    from EnumProv in XMLDocElements.Elements("Province")
                    where (string)EnumProv.Attribute("Name") == ProvinceName
                    select EnumProv;

                IEnumerable<XElement> Cities =
                    from EnumCity in ProvinceNode.Descendants()
                    select EnumCity;

                foreach (XElement EnumCity in Cities)
                {

                    string CityName = EnumCity.Value;
                    CityNameList.Add(CityName);
                    CityNameList.Sort();
                }
            }
            
        }
        public List<string> ReadProvinceList()
        {
            List<string> ProvinceList = new List<string>();
            if (XMLDocElements != null)
            {
                IEnumerable<XElement> Provinces =
                    from EnumProv in XMLDocElements.Elements("Province")
                    select EnumProv;

                IEnumerable<XAttribute> attList =
                    from at in Provinces.Attributes()
                    select at;

                foreach (XAttribute EnumProv in attList)
                {
                    ProvinceList.Add(EnumProv.Value);
                }
            }

            return ProvinceList;
        }

        public string GetCityCode(string CityName)
        {
            XElement City, CityCode;
            
            if (XMLDocElements != null)
            {
                IEnumerable<XElement> CityNode =
                    from EnumProv in XMLDocElements.Elements("City")
                    where (string)EnumProv.Attribute("Name") == CityName
                    select EnumProv;

                City = CityNode.First();
                CityCode = (XElement)City.FirstNode;
                                                
                return CityCode.Value.ToString();
            }

            return "Failed";
        }

        public string GetHomeURL(string CityName)
        {
            XElement City, BaseURL;

            if (XMLDocElements != null)
            {
                IEnumerable<XElement> CityNode =
                    from EnumProv in XMLDocElements.Elements("City")
                    where (string)EnumProv.Attribute("Name") == CityName
                    select EnumProv;

                City = CityNode.First();
                BaseURL = (XElement)City.LastNode;

                return BaseURL.Value.ToString();
            }

            return "Failed";
        }

        public void SetSourceFile(string FileName)
        {
            try
            {
                XMLDocElements = XElement.Load(FileName);
            }
            catch (Exception e)
            {

            }
        }
    }
}
