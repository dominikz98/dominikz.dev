using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace packages.ColorCode.Tests.TestData
{
    public class JavaKeywordData : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo methodUnderTest)
        {
            var keywordData = new List<object[]>
                                  {
                                      new object[] {"abstract"},
                                      new object[] {"assert"},
                                      new object[] {"boolean"},
                                      new object[] {"break"},
                                      new object[] {"byte"},
                                      new object[] {"case"},
                                      new object[] {"catch"},
                                      new object[] {"char"},
                                      new object[] {"class"},
                                      new object[] {"const"},
                                      new object[] {"continue"},
                                      new object[] {"default"},
                                      new object[] {"do"},
                                      new object[] {"double"},
                                      new object[] {"else"},
                                      new object[] {"enum"},
                                      new object[] {"extends"},
                                      new object[] {"false"},
                                      new object[] {"final"},
                                      new object[] {"finally"},
                                      new object[] {"float"},
                                      new object[] {"for"},
                                      new object[] {"goto"},
                                      new object[] {"if"},
                                      new object[] {"implements"},
                                      new object[] {"import"},
                                      new object[] {"instanceof"},
                                      new object[] {"int"},
                                      new object[] {"interface"},
                                      new object[] {"long"},
                                      new object[] {"native"},
                                      new object[] {"new"},
                                      new object[] {"null"},
                                      new object[] {"package"},
                                      new object[] {"private"},
                                      new object[] {"protected"},
                                      new object[] {"public"},
                                      new object[] {"return"},
                                      new object[] {"short"},
                                      new object[] {"static"},
                                      new object[] {"strictfp"},
                                      new object[] {"super"},
                                      new object[] {"switch"},
                                      new object[] {"synchronized"},
                                      new object[] {"this"},
                                      new object[] {"throw"},
                                      new object[] {"throws"},
                                      new object[] {"transient"},
                                      new object[] {"true"},
                                      new object[] {"try"},
                                      new object[] {"void"},
                                      new object[] {"volatile"},
                                      new object[] {"while"}
                                  };
            return keywordData;
        }
    }
}
