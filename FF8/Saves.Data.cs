﻿using System;
using System.IO;

namespace FF8
{

    internal static partial class Saves
    {
        public class Data
        {
            public ushort LocationID;//0x0004
            public ushort firstcharacterscurrentHP;//0x0006
            public ushort firstcharactersmaxHP;//0x0008
            public ushort savecount;//0x000A
            public uint AmountofGil;//0x000C

            /// <summary>
            /// Stored playtime in seconds. Made into timespan for easy parsing.
            /// </summary>
            public TimeSpan timeplayed;//0x0020

            public byte firstcharacterslevel;//0x0024

            /// <summary>
            /// 0xFF = blank; The value should cast to Faces.ID
            /// </summary>
            public Faces.ID[] charactersportraits;//0x0025//0x0026//0x0027

            /// <summary>
            /// 12 characters 0x00 terminated
            /// </summary>
            public FF8String Squallsname;//0x0028 //12 characters 0x00 terminated

            public FF8String Rinoasname;//0x0034 //12 characters 0x00 terminated
            public FF8String Angelosname;//0x0040 //12 characters 0x00 terminated
            public FF8String Bokosname;//0x004C //12 characters 0x00 terminated

            // 0 = Disc 1
            public uint CurrentDisk;//0x0058

            public uint Currentsave;//0x005C

            public GFData[] GFs; // 0x0060 -> 0x045C //68 bytes per 16 total
            public CharacterData[] Characters; // 0x04A0 -> 0x08C8 //152 bytes per 8 total
            public byte[] Shops; //0x0960 //400 bytes
            public byte[] Configuration; //0x0AF0 //20 bytes
            public Faces.ID[] Party; //0x0B04 // 4 bytes 0xFF terminated.
            public byte[] KnownWeapons; //0x0B08 // 4 bytes
            public FF8String Grieversname; //0x0B0C // 12 bytes

            public ushort Unknown1; //0x0B18  (always 7966?)
            public ushort Unknown2; //0x0B1A
            public uint AmountofGil2; //0x0B1C
            public uint AmountofGil_Laguna; //0x0B20
            public ushort LimitBreakQuistis; //0x0B24
            public ushort LimitBreakZell; //0x0B26
            public byte LimitBreakIrvine; //0x0B28
            public byte LimitBreakSelphie; //0x0B29
            public byte LimitBreakAngelocompleted; //0x0B2A
            public byte LimitBreakAngeloknown; //0x0B2B
            public byte[] LimitBreakAngelopoints; //0x0B2C
            public byte[] Itemsbattleorder; //0x0B34
            public Item[] Items; //0x0B54 198 items (Item ID and Quantity)
            public TimeSpan Gametime; //0x0CE0
            public uint Countdown; //0x0CE4
            public uint Unknown3; //0x0CE8
            public uint Battlevictorycount; //0x0CEC
            public ushort Unknown4; //0x0CF0
            public ushort Battlebattleescaped; //0x0CF2
            public uint Unknown5; //0x0CF4
            public uint BattleTonberrykilledcount; //0x0CF8
            public bool BattleTonberrySrkilled; //0x0CFC (yeah, this is a boolean)
            public uint Unknown6; //0x0D00
            public uint BattleR1; //0x0D04 First "Bug" battle (R1 tip)
            public uint BattleELEMENTAL; //0x0D08 First "Bomb" battle (Elemental tip)
            public uint BattleMENTAL; //0x0D0C  First "T-Rex" battle (Mental tip)
            public uint BattleIRVINE; //0x0D10 First "Irvine" battle (Irvine's limit break tip)
            public byte[] BattleMAGIC; //0x0D14 Magic drawn once
            public byte[] BattleSCAN; //0x0D1C Ennemy scanned once
            public byte BattleRAUTO; //0x0D30 Renzokuken auto
            public byte BattleRINDICATOR; //0x0D31 Renzokuken indicator
            public byte BattleUNK; //0x0D32 dream/Odin/Phoenix/Gilgamesh/Angelo disabled/Angel Wing enabled/???/???
            public byte[] Tutorialinfos; //0x0D33
            public byte SeeDtestlevel; //0x0D43
            public uint Unknown; //0x0D44
            public uint Party2; //0x0D48 (last byte always = 255)
            public uint Unknown7; //0x0D4C
            public ushort Module; //0x0D50 (1= field, 2= worldmap, 3= battle)
            public ushort Currentfield; //0x0D52
            public ushort Previousfield; //0x0D54
            public short[] CoordX; //0x0D56 signed  (party1, party2, party3)
            public short[] CoordY; //0x0D5C signed  (party1, party2, party3)
            public ushort[] Triangle_ID; //0x0D62  (party1, party2, party3)
            public byte[] Direction; //0x0D68  (party1, party2, party3)
            public byte Padding; //0x0D6B
            public uint Unknown8; //0x0D6C
            public byte[] Fieldvars; //0x0D70 http://wiki.ffrtt.ru/index.php/FF8/Variables
            public byte[] Worldmap; //0x1270
            public byte[] TripleTriad; //0x12F0
            public byte[] ChocoboWorld; //0x1370

            public Data()
            {
                LocationID = 0;
                firstcharacterscurrentHP = 0;
                firstcharactersmaxHP = 0;
                savecount = 0;
                AmountofGil = 0;
                timeplayed = new TimeSpan();
                firstcharacterslevel = 0;
                charactersportraits = null;
                Squallsname = null;
                Rinoasname = null;
                Angelosname = null;
                Bokosname = null;
                CurrentDisk = 0;
                Currentsave = 0;
                GFs = new GFData[16];
                Characters = new CharacterData[8];
                Shops = null;
                Configuration = null;
                Party = null;
                KnownWeapons = null;
                Grieversname = null;
            }

            public struct Item { public byte ID; public byte QTY; };
            public void Read(BinaryReader br)
            {
                LocationID = br.ReadUInt16();//0x0004
                firstcharacterscurrentHP = br.ReadUInt16();//0x0006
                firstcharactersmaxHP = br.ReadUInt16();//0x0008
                savecount = br.ReadUInt16();//0x000A
                AmountofGil = br.ReadUInt32();//0x000C
                timeplayed = new TimeSpan(0, 0, (int)br.ReadUInt32());//0x0020
                firstcharacterslevel = br.ReadByte();//0x0024
                charactersportraits = Array.ConvertAll(br.ReadBytes(3), Item => (Faces.ID)Item);//0x0025//0x0026//0x0027 0xFF = blank.
                Squallsname = br.ReadBytes(12);//0x0028
                Rinoasname = br.ReadBytes(12);//0x0034
                Angelosname = br.ReadBytes(12);//0x0040
                Bokosname = br.ReadBytes(12);//0x004C
                CurrentDisk = br.ReadUInt32();//0x0058
                Currentsave = br.ReadUInt32();//0x005C
                for (int i = 0; i < GFs.Length; i++)
                {
                    GFs[i].Read(br);
                }
                for (int i = 0; i <= (int)Faces.ID.Edea_Kramer; i++)
                {
                    Characters[i].Read(br); // 0x04A0 -> 0x08C8 //152 bytes per 8 total
                    Characters[i].Name = Memory.Strings.GetName((Faces.ID)i,this);
                }
                Shops = br.ReadBytes(400); //0x0960 //400 bytes
                Configuration = br.ReadBytes(20); //0x0AF0 //20 bytes
                Party = charactersportraits;
                br.BaseStream.Seek(4, SeekOrigin.Current);//0x0B04 // 4 bytes 0xFF terminated.
                KnownWeapons = br.ReadBytes(4); //0x0B08 // 4 bytes
                Grieversname = br.ReadBytes(12); //0x0B0C // 12 bytes

                Unknown = br.ReadUInt16();//0x0B18  (always 7966?)
                Unknown = br.ReadUInt16();//0x0B1A 
                AmountofGil2 = br.ReadUInt32();//0x0B1C //dupilicate
                AmountofGil_Laguna = br.ReadUInt32();//0x0B20 
                LimitBreakQuistis = br.ReadUInt16();//0x0B24 
                LimitBreakZell = br.ReadUInt16();//0x0B26 
                LimitBreakIrvine = br.ReadByte();//0x0B28 
                LimitBreakSelphie = br.ReadByte();//0x0B29 
                LimitBreakAngelocompleted = br.ReadByte();//0x0B2A 
                LimitBreakAngeloknown = br.ReadByte();//0x0B2B 
                LimitBreakAngelopoints = br.ReadBytes(8);//0x0B2C 
                Itemsbattleorder = br.ReadBytes(32);//0x0B34
                Items = new Item[198];
                for (int i = 0; i < 198; i++)
                    Items[0] = new Item { ID = br.ReadByte(), QTY = br.ReadByte() }; //0x0B54 198 items (Item ID and Quantity)
                Gametime = new TimeSpan(0, 0, (int)br.ReadUInt32());//0x0CE0 
                Countdown = br.ReadUInt32();//0x0CE4 
                Unknown = br.ReadUInt32();//0x0CE8 
                Battlevictorycount = br.ReadUInt32();//0x0CEC 
                Unknown = br.ReadUInt16();//0x0CF0 
                Battlebattleescaped = br.ReadUInt16();//0x0CF2 
                Unknown = br.ReadUInt32();//0x0CF4 
                BattleTonberrykilledcount = br.ReadUInt32();//0x0CF8 
                BattleTonberrySrkilled = br.ReadUInt32()>0;//0x0CFC (yeah, this is a boolean)
                Unknown = br.ReadUInt32();//0x0D00 
                BattleR1 = br.ReadUInt32();//0x0D04 First "Bug" battle (R1 tip)
                BattleELEMENTAL = br.ReadUInt32();//0x0D08 First "Bomb" battle (Elemental tip)
                BattleMENTAL = br.ReadUInt32();//0x0D0C  First "T-Rex" battle (Mental tip)
                BattleIRVINE = br.ReadUInt32();//0x0D10 First "Irvine" battle (Irvine's limit break tip)
                BattleMAGIC = br.ReadBytes(8);//0x0D14 Magic drawn once
                BattleSCAN = br.ReadBytes(20);//0x0D1C Ennemy scanned once
                BattleRAUTO = br.ReadByte();//0x0D30 Renzokuken auto 
                BattleRINDICATOR = br.ReadByte();//0x0D31 Renzokuken indicator
                BattleUNK = br.ReadByte();//0x0D32 dream/Odin/Phoenix/Gilgamesh/Angelo disabled/Angel Wing enabled/???/???
                Tutorialinfos = br.ReadBytes(16);//0x0D33 
                SeeDtestlevel = br.ReadByte();//0x0D43 
                Unknown = br.ReadUInt32();//0x0D44 
                Party2 = br.ReadUInt32();//0x0D48 (last byte always = 255) //dupicate
                Unknown = br.ReadUInt32();//0x0D4C 
                Module = br.ReadUInt16();//0x0D50 (1= field, 2= worldmap, 3= battle)
                Currentfield = br.ReadUInt16();//0x0D52 
                Previousfield = br.ReadUInt16();//0x0D54 
                CoordX = new short[3];
                for (int i =0; i<3;i++)
                    CoordX[i] = br.ReadInt16();//0x0D56 signed  (party1, party2, party3)
                CoordY = new short [3];
                for (int i = 0; i < 3; i++)
                    CoordY[i] = br.ReadInt16();//0x0D5C signed  (party1, party2, party3)
                Triangle_ID = new ushort[3];
                for (int i = 0; i < 3; i++)
                    Triangle_ID[i] = br.ReadUInt16();//0x0D62  (party1, party2, party3)
                Direction = br.ReadBytes(3 * 1);//0x0D68  (party1, party2, party3)
                Padding = br.ReadByte();//0x0D6B 
                Unknown = br.ReadUInt32();//0x0D6C 
                //Fieldvars = br.ReadBytes(256 + 1024);//0x0D70 http://wiki.ffrtt.ru/index.php/FF8/Variables
                //Worldmap = br.ReadBytes(128);//0x1270 
                //TripleTriad = br.ReadBytes(128);//0x12F0 
                //ChocoboWorld = br.ReadBytes(64);//0x1370 

            }
        }
    }
}