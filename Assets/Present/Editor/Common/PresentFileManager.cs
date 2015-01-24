using System.IO;
using SevenZip.Compression.LZMA;
using SevenZip;
using System;

public class PresentFileManager
{
    static PresentFileManager() { Instance = new PresentFileManager(); }

    private PresentFileManager() { }

    public static PresentFileManager Instance { get; private set; }
}