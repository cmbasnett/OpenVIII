﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FF8
{
    internal abstract class SP2 //: I_SP2
    {
        protected SP2()
        {
            Count = 0;
            PalletCount = 1;
            TextureCount = 1;
            EntriesPerTexture = 1;
            TextureFilename = "";
            Scale = Vector2.One;
            TextureStartOffset = 0;
            IndexFilename = "";
            Textures = null;
            Entries = null;
        }

        /// <summary>
        /// enum to be added to class when implemented
        /// </summary>
        public enum ID { NotImplemented }
        /// <summary>
        /// Number of Entries
        /// </summary>
        public uint Count { get; protected set; }

        /// <summary>
        /// Number of Pallets
        /// </summary>
        public uint PalletCount { get; protected set; }

        /// <summary>
        /// Number of Textures
        /// </summary>
        public int TextureCount { get; protected set; }

        /// <summary>
        /// Entries per texture,ID MOD EntriesPerTexture to get current entry to use on this texture
        /// </summary>
        public uint EntriesPerTexture { get; protected set; }

        /// <summary>
        /// Texture filename. To match more than one number use {0:00} or {00:00} for ones with
        /// leading zeros.
        /// </summary>
        /// TODO make array maybe to add support for highres versions. the array will need to come
        /// with a scale factor
        protected string TextureFilename { get; set; }

        /// <summary>
        /// Should be Vector2.One unless reading a high res version of textures.
        /// </summary>
        protected Vector2 Scale { get; set; }

        /// <summary>
        /// Some textures start with 1 and some start with 0. This is added to the current number in
        /// when reading the files in.
        /// </summary>
        protected int TextureStartOffset { get; set; }

        /// <summary>
        /// *.sp1 or *.sp2 that contains the entries or entrygroups. With Rectangle and offset information.
        /// </summary>
        protected string IndexFilename { get; set; }

        /// <summary>
        /// List of textures
        /// </summary>
        protected virtual List<Texture2D> Textures { get; set; }

        /// <summary>
        /// Dictionary of Entries
        /// </summary>
        protected virtual Dictionary<uint, Entry> Entries { get; set; }

        /// <summary>
        /// Draw Item
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dst"></param>
        /// <param name="fade"></param>
        public virtual void Draw(Enum id, Rectangle dst, float fade = 1)
        {
            Memory.spriteBatch.Draw(GetTexture(id), dst, GetEntry(id).GetRectangle, Color.White * fade);
        }
        public Entry this[Enum id] => GetEntry(id);
        public virtual Entry GetEntry(Enum id)
        {
            if (Entries.ContainsKey(Convert.ToUInt32(id)))
                return Entries[Convert.ToUInt32(id)];
            if (Entries.ContainsKey(Convert.ToUInt32(id) % EntriesPerTexture))
                return Entries[Convert.ToUInt32(id) % EntriesPerTexture];
            return null;
        }
        public virtual Texture2D GetTexture(Enum id)
        {
            uint pos = Convert.ToUInt32(id);
            uint File = GetEntry(id).File;
            //check if we set a custom file and we have a pos more then set entriespertexture
            if (File == 0 && pos > EntriesPerTexture)
                File = pos / EntriesPerTexture;
            if (File >= TextureCount)
                File %= (uint)TextureCount;
            return Textures[(int)File];
        }

        public virtual void Init()
        {
            if (Entries == null)
            {
                Textures = new List<Texture2D>(TextureCount);
                ArchiveWorker aw = new ArchiveWorker(Memory.Archives.A_MENU);
                using (MemoryStream ms = new MemoryStream(ArchiveWorker.GetBinaryFile(Memory.Archives.A_MENU,
                    aw.GetListOfFiles().First(x => x.ToLower().Contains(IndexFilename)))))
                {
                    ushort[] locs;
                    using (BinaryReader br = new BinaryReader(ms))
                    {
                        //ms.Seek(4, SeekOrigin.Begin);
                        Count = br.ReadUInt32();
                        locs = new ushort[Count];//br.ReadUInt32(); 32 valid values in face.sp2 rest is invalid
                        Entries = new Dictionary<uint, Entry>((int)Count);
                        for (uint i = 0; i < Count; i++)
                        {
                            locs[i] = br.ReadUInt16();
                            ms.Seek(2, SeekOrigin.Current);
                        }
                        byte fid = 0;
                        Entry Last = null;
                        for (uint i = 0; i < Count; i++)
                        {
                            ms.Seek(locs[i] + 6, SeekOrigin.Begin);
                            byte t = br.ReadByte();
                            if (t == 0 || t == 96) // known invalid entries in sp2 files have this value. there might be more to it.
                            {
                                Count = i;
                                break;
                            }

                            Entries[i] = new Entry();
                            Entries[i].LoadfromStreamSP2(br,locs[i], Last, ref fid);

                            Last = Entries[i];
                        }
                    }
                }
                //using (FileStream fs = File.OpenWrite(Path.Combine("d:\\", "face.sp2")))
                //{
                //    fs.Write(test, 0, test.Length);
                //}
                TEX tex;
                for (int i = 0; i < TextureCount; i++)
                {
                    tex = new TEX(ArchiveWorker.GetBinaryFile(Memory.Archives.A_MENU,
                        aw.GetListOfFiles().First(x => x.ToLower().Contains(string.Format(TextureFilename, i + TextureStartOffset)))));
                    Textures.Add(tex.GetTexture());
                }
            }
        }
    }
}