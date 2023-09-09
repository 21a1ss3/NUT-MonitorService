using Microsoft.VisualStudio.TestTools.UnitTesting;
using IO=System.IO;
using Vmodl = NUTMonitor.Esxi.Vmodl;

namespace NUT_MonitorService.Tests
{
    [TestClass]
    public class VmodlTests
    {
        [TestMethod]
        public void TestString()
        {
            string content = "Demo string!!";
            string vmodl = $"\"{content}\"";
            Vmodl.VmodlReader vmodlReader = new Vmodl.VmodlReader(vmodl);

            Assert.IsTrue(vmodlReader.Parse());
            Assert.AreEqual(Vmodl.VmodlToken.String, vmodlReader.TokenType);
            Assert.AreEqual(content, vmodlReader.RawValue);

            Assert.IsTrue(vmodlReader.Parse());
            Assert.AreEqual(Vmodl.VmodlToken.EOF, vmodlReader.TokenType);
            Assert.AreEqual(string.Empty, vmodlReader.RawValue);
        }

        [TestMethod]
        public void TestBoolTrue()
        {
            string token = "true";
            string vmodl = $"{token}";
            Vmodl.VmodlReader vmodlReader = new Vmodl.VmodlReader(vmodl);

            Assert.IsTrue(vmodlReader.Parse());
            Assert.AreEqual(Vmodl.VmodlToken.True, vmodlReader.TokenType);
            Assert.AreEqual(token, vmodlReader.RawValue);

            Assert.IsTrue(vmodlReader.Parse());
            Assert.AreEqual(Vmodl.VmodlToken.EOF, vmodlReader.TokenType);
            Assert.AreEqual(string.Empty, vmodlReader.RawValue);
        }

        [TestMethod]
        public void TestBoolFalse()
        {
            string token = "false";
            string vmodl = $"{token}";
            Vmodl.VmodlReader vmodlReader = new Vmodl.VmodlReader(vmodl);

            Assert.IsTrue(vmodlReader.Parse());
            Assert.AreEqual(Vmodl.VmodlToken.False, vmodlReader.TokenType);
            Assert.AreEqual(token, vmodlReader.RawValue);

            Assert.IsTrue(vmodlReader.Parse());
            Assert.AreEqual(Vmodl.VmodlToken.EOF, vmodlReader.TokenType);
            Assert.AreEqual(string.Empty, vmodlReader.RawValue);
        }


        [TestMethod]
        [DeploymentItem("SimpleSample1.vmodl", "vmodl")]
        public void ParseSimpleSample1()
        {
            string vmodl = IO.File.ReadAllText("vmodl\\SimpleSample1.vmodl");
            Vmodl.VmodlReader vmodlReader = new Vmodl.VmodlReader(vmodl);

            Assert.IsTrue(vmodlReader.Parse());
            Assert.AreEqual(Vmodl.VmodlToken.StartTypeName, vmodlReader.TokenType);
            Assert.AreEqual("(",vmodlReader.RawValue);

            Assert.IsTrue(vmodlReader.Parse());
            Assert.AreEqual(Vmodl.VmodlToken.TypeName,vmodlReader.TokenType);
            Assert.AreEqual("type.name.and.namespace",vmodlReader.RawValue);

            Assert.IsTrue(vmodlReader.Parse());
            Assert.AreEqual(Vmodl.VmodlToken.EndTypeName,vmodlReader.TokenType);
            Assert.AreEqual(")",vmodlReader.RawValue);

            Assert.IsTrue(vmodlReader.Parse());
            Assert.AreEqual(Vmodl.VmodlToken.StartObject,vmodlReader.TokenType);
            Assert.AreEqual("{",vmodlReader.RawValue);



            Assert.IsTrue(vmodlReader.Parse());
            Assert.AreEqual(Vmodl.VmodlToken.PropertyName, vmodlReader.TokenType);
            Assert.AreEqual("propname",vmodlReader.RawValue );

            Assert.IsTrue(vmodlReader.Parse());
            Assert.AreEqual(Vmodl.VmodlToken.String, vmodlReader.TokenType);
            Assert.AreEqual("string value", vmodlReader.RawValue);



            Assert.IsTrue(vmodlReader.Parse());
            Assert.AreEqual(Vmodl.VmodlToken.PropertyName, vmodlReader.TokenType);
            Assert.AreEqual("UnsetProp", vmodlReader.RawValue);

            Assert.IsTrue(vmodlReader.Parse());
            Assert.AreEqual(Vmodl.VmodlToken.Unset, vmodlReader.TokenType);
            Assert.AreEqual("<unset>", vmodlReader.RawValue);



            Assert.IsTrue(vmodlReader.Parse());
            Assert.AreEqual(Vmodl.VmodlToken.PropertyName, vmodlReader.TokenType);
            Assert.AreEqual("NullProp", vmodlReader.RawValue);


            Assert.IsTrue(vmodlReader.Parse());
            Assert.AreEqual(Vmodl.VmodlToken.StartTypeName, vmodlReader.TokenType);
            Assert.AreEqual("(", vmodlReader.RawValue);

            Assert.IsTrue(vmodlReader.Parse());
            Assert.AreEqual(Vmodl.VmodlToken.TypeName, vmodlReader.TokenType);
            Assert.AreEqual("someType", vmodlReader.RawValue);

            Assert.IsTrue(vmodlReader.Parse());
            Assert.AreEqual(Vmodl.VmodlToken.EndTypeName, vmodlReader.TokenType);
            Assert.AreEqual(")", vmodlReader.RawValue);

            Assert.IsTrue(vmodlReader.Parse());
            Assert.AreEqual(Vmodl.VmodlToken.Null, vmodlReader.TokenType);
            Assert.AreEqual("null", vmodlReader.RawValue);


            
            Assert.IsTrue(vmodlReader.Parse());
            Assert.AreEqual(Vmodl.VmodlToken.PropertyName, vmodlReader.TokenType);
            Assert.AreEqual("IntProp", vmodlReader.RawValue);

            Assert.IsTrue(vmodlReader.Parse());
            Assert.AreEqual(Vmodl.VmodlToken.Integer, vmodlReader.TokenType);
            Assert.AreEqual("5", vmodlReader.RawValue);


            Assert.IsTrue(vmodlReader.Parse());
            Assert.AreEqual(Vmodl.VmodlToken.EndObject, vmodlReader.TokenType);
            Assert.AreEqual("}", vmodlReader.RawValue);


            Assert.IsTrue(vmodlReader.Parse());
            Assert.AreEqual(Vmodl.VmodlToken.EOF, vmodlReader.TokenType);
            Assert.AreEqual(string.Empty, vmodlReader.RawValue);

        }
        [TestMethod]
        [DeploymentItem("ComplexSample1.vmodl", "vmodl")]
        public void ParseComplexSample1()
        {
            string vmodle = IO.File.ReadAllText("vmodl\\ComplexSample1.vmodl");


            //SimpleSample1.vmodl
        }
    }
}
