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
        public void InitialiserTest()
        {
            var maker = new TileMaker(PATH);
            Assert.IsTrue(Directory.Exists("../../TestFiles/Win10TileMaker_Assets"));
        }

        [TestMethod]
        public void SetLogoPathTest()
        {
            var maker = new TileMaker(PATH);
            maker.SetSquareLogo("../../TestFiles/Miku.png");
            Assert.IsTrue(File.Exists("../../TestFiles/Win10TileMaker_Assets/SquareLogo.png"));
        }

        [TestMethod]
        public void WriteManifestTest()
        {
            var maker = new TileMaker(PATH);
            maker.SetSquareLogo("../../TestFiles/Miku.png");
            maker.WriteManifest();
            Assert.IsTrue(File.Exists("../../TestFiles/HelloTileMaker.VisualElementsManifest.xml"));
        }

        [TestMethod]
        public void MakeSumatraTileTest()
        {
            var maker = new TileMaker(@"D:\Application\SumatraPDF\SumatraPDF.exe");
            maker.SetSquareLogo(@"D:\Application\SumatraPDF\150x150Logo.png");
            maker.MakeTile();
            Assert.IsTrue(File.Exists(@"D:\Application\SumatraPDF\SumatraPDF.VisualElementsManifest.xml"));
        }

        [TestMethod]
        public void CreateShortcutTest()
        {
            var maker = new TileMaker(PATH);
            maker.CreateShortcut();
            Assert.IsTrue(File.Exists(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\HelloTileMaker.lnk"));
        }

        [TestMethod]
        public void MakeTileTest()
        {
            var maker = new TileMaker(PATH);
            maker.SetSquareLogo("../../TestFiles/Miku.png");
            maker.MakeTile();
            Assert.IsTrue(File.Exists("../../TestFiles/HelloTileMaker.VisualElementsManifest.xml"));
            Assert.IsTrue(File.Exists(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\HelloTileMaker.lnk"));
        }

        [TestMethod]
        public void RemoveCustomizationTest()
        {
            var maker = new TileMaker(PATH);
            maker.RemoveCustomization(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\HelloTileMaker.lnk");
            Assert.IsTrue(!File.Exists("../../TestFiles/HelloTileMaker.VisualElementsManifest.xml-wtm"));
            Assert.IsTrue(!File.Exists("../../TestFiles/Win10TileMaker_Assets"));
        }
    }
}
