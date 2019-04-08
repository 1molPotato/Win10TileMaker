using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using TileMakerLibrary;

namespace TileMakerTest
{
    [TestClass]
    public class TileMakerTest
    {
        const string PATH = "../../TestFiles/HelloTileMaker.exe";

        [TestMethod]
        public void CreateBackupTest()
        {
            var maker = new TileMaker(PATH);
            maker.SetSquareLogo("../../TestFiles/Miku.png");
            maker.MakeTile();
            Directory.Delete("../../TestFiles/Win10TileMaker_Assets", true);
            maker.MakeTile();
            Assert.IsTrue(File.Exists("../../TestFiles/HelloTileMaker.VisualElementsManifest.xml-wtm"));
        }

        [TestMethod]
        public void CreateAssetsTest()
        {
            var maker = new TileMaker(PATH);
            maker.SetSquareLogo("../../TestFiles/Miku.png");
            maker.MakeTile();
            Assert.IsTrue(Directory.Exists("../../TestFiles/Win10TileMaker_Assets"));
            Assert.IsTrue(File.Exists("../../TestFiles/Win10TileMaker_Assets/150x150Logo.png"));
            Assert.IsTrue(File.Exists("../../TestFiles/Win10TileMaker_Assets/70x70Logo.png"));
        }

        [TestMethod]
        public void CreateManifestTest()
        {
            var maker = new TileMaker(PATH);
            maker.SetSquareLogo("../../TestFiles/Miku.png");
            maker.MakeTile();
            Assert.IsTrue(File.Exists("../../TestFiles/HelloTileMaker.VisualElementsManifest.xml"));
        }

        [TestMethod]
        public void CreateShortcutTest()
        {
            var maker = new TileMaker(PATH);
            maker.SetSquareLogo("../../TestFiles/Miku.png");
            maker.MakeTile();
            Assert.IsTrue(File.Exists(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\HelloTileMaker.lnk"));
        }     

        [TestMethod]
        public void RemoveCustomizationTest()
        {
            var maker = new TileMaker(PATH);
            maker.SetSquareLogo("../../TestFiles/Miku.png");
            maker.MakeTile();
            maker.RemoveCustomization();
            Assert.IsTrue(!File.Exists("../../TestFiles/HelloTileMaker.VisualElementsManifest.xml-wtm"));
            Assert.IsTrue(!File.Exists("../../TestFiles/Win10TileMaker_Assets"));
        }
    }

    [TestClass]
    public class UtilitiesTest
    {
        [TestMethod]
        public void GetTargetPathTest()
        {
            string expected = new FileInfo("../../TestFiles/HelloTileMaker.exe").FullName;
            string actual = new FileInfo(Utilities.GetTargetPath(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\HelloTileMaker.lnk")).FullName;
            Assert.AreEqual(expected, actual);
        }
    }
}
