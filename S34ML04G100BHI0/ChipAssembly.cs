using NAND_Prog;
using System;
using System.ComponentModel.Composition;

namespace S34ML04G100BH100
{
    /*
     use the design :

      # region
         <some code> 
      # endregion

    for automatically include <some code> in the READMY.md file in the repository
    */

   
    public class ChipAssembly
    {
        [Export("Chip")]
        ChipPrototype myChip = new ChipPrototype();



        #region Chip parameters

        //--------------------Vendor Specific Pin configuration---------------------------

        //  VSP1(38pin) - NC    
        //  VSP2(35pin) - NC
        //  VSP3(20pin) - NC

        ChipAssembly()
        {
            myChip.devManuf = "Spansion";
            myChip.name = "S34ML04G100BH100";
            myChip.chipID = "01DC909554";      // device ID - 01h/DCh/90h/95h/54h

            myChip.width = Organization.x8;    // chip width - 8 bit
            myChip.bytesPP = 2048;             // page size - 2048 byte (2Kb)
            myChip.spareBytesPP = 64;          // size Spare Area - 64 byte
            myChip.pagesPB = 64;               // the number of pages per block - 64 
            myChip.bloksPLUN = 4096;           // number of blocks in CE - 4096
            myChip.LUNs = 1;                   // the amount of CE in the chip
            myChip.colAdrCycles = 2;           // cycles for column addressing
            myChip.rowAdrCycles = 3;           // cycles for row addressing 
            myChip.vcc = Vcc.v3_3;             // supply voltage

        #endregion


            #region Chip operations

            //------- Add chip operations    https://github.com/JuliProg/Wiki#command-set----------------------------------------------------

            myChip.Operations("Reset_FFh").
                   Operations("Erase_60h_D0h").
                   Operations("Read_00h_30h").
                   Operations("PageProgram_80h_10h");

            #endregion
                

            #region Initial Invalid Block (s)
            
            //------- Select the Initial Invalid Block (s) algorithm    https://github.com/JuliProg/Wiki/wiki/Initiate-Invalid-Block-----------
                
            myChip.InitialInvalidBlock = "InitInvalidBlock_v2";
                
            #endregion
                
                

            #region Chip registers (optional)

            //------- Add chip registers (optional)----------------------------------------------------

            myChip.registers.Add(                   // https://github.com/JuliProg/Wiki/wiki/StatusRegister
                "Status Register").
                Size(1).
                Operations("ReadStatus_70h").
                Interpretation("SR_Interpreted").
                UseAsStatusRegister();



            myChip.registers.Add(                  // https://github.com/JuliProg/Wiki/wiki/ID-Register
                "Id Register").
                Size(5).
                Operations("ReadId_90h");
            //Interpretation(ID_interpreting);

            myChip.registers.Add(
              "Parameter Page (ONFI parameter)").
              Size(768).
              Operations("ReadParameterPage_ECh");

            #endregion


        }

        #region Interpretation of ID-register values ​​(optional)

        public string ID_interpreting(Register register)   
        
        #endregion
        {
            byte[] content = register.GetContent();
            string messsage = String.Empty;


           // if (BitConverter.ToString(content).Replace("-", "") != "ECF1001540")
           // {
           //     messsage += "This chip is not a S34ML04G100BHI0";
           //     return messsage;
           // }
           
            
            
            messsage += "1st Byte    Maker Code = " + content[0].ToString("X2") + Environment.NewLine;
            messsage += ID_decoding(content[0],0) + Environment.NewLine;

            messsage += "2nd Byte    Device Code = " + content[1].ToString("X2") + Environment.NewLine;
            messsage += ID_decoding(content[1], 1) + Environment.NewLine;

            messsage += "3rd ID Data = " + content[2].ToString("X2") + Environment.NewLine;
            messsage += ID_decoding(content[2], 2) + Environment.NewLine;

            messsage += "4th ID Data = " + content[3].ToString("X2") + Environment.NewLine;
            messsage += ID_decoding(content[3], 3) + Environment.NewLine;

            messsage += "5th ID Data = " + content[4].ToString("X2") + Environment.NewLine;
            messsage += ID_decoding(content[4], 4) + Environment.NewLine;

            return messsage;
        }  
        private string ID_decoding(byte bt, int pos)
        {
            string str_result = String.Empty;

            var IO = new System.Collections.BitArray(new[] { bt });

            switch (pos)
            {
                case 0:
                    str_result += "Maker ";
                    if (bt == 0xEC)
                        str_result += "is Samsung";
                    else
                        str_result += "is not Samsung";
                    str_result += Environment.NewLine;
                    break;

                case 1:
                    str_result += "Device ";
                    if (bt == 0xF1)
                        str_result += "is S34ML04G100BHI0";
                    else
                        str_result += "is not S34ML04G100BHI0";
                    str_result += Environment.NewLine;
                    break;

                case 2:
                    str_result += " Internal Chip Number = ";
                    if (IO[1] == false && IO[0] == false)
                        str_result += "1";
                    if (IO[1] == false && IO[0] == true)
                        str_result += "2";
                    if (IO[1] == true && IO[0] == false)
                        str_result += "4";
                    if (IO[1] == true && IO[0] == true)
                        str_result += "8";
                    str_result += Environment.NewLine;


                    str_result += " Cell Type = ";
                    if (IO[3] == false && IO[2] == false)
                        str_result += "2 Level Cell";
                    if (IO[3] == false && IO[2] == true)
                        str_result += "4 Level Cell";
                    if (IO[3] == true && IO[2] == false)
                        str_result += "8 Level Cell";
                    if (IO[3] == true && IO[2] == true)
                        str_result += "16 Level Cell";
                    str_result += Environment.NewLine;


                    str_result += " Number of Simultaneously Programmed Pages = ";
                    if (IO[5] == false && IO[4] == false)
                        str_result += "1";
                    if (IO[5] == false && IO[4] == true)
                        str_result += "2";
                    if (IO[5] == true && IO[4] == false)
                        str_result += "4";
                    if (IO[5] == true && IO[4] == true)
                        str_result += "8";
                    str_result += Environment.NewLine;


                    str_result += " Interleave Program Between multiple chips = ";
                    if (IO[6] == false)
                        str_result += "Not Support";
                    if (IO[6] == true)
                        str_result += "Support";
                    str_result += Environment.NewLine;

                    str_result += " Cache Program = ";
                    if (IO[7] == false)
                        str_result += "Not Support";
                    if (IO[7] == true)
                        str_result += "Support";
                    str_result += Environment.NewLine;
                    break;

                case 3:

                    str_result += " Page Size (w/o redundant area ) = ";
                    if (IO[1] == false && IO[0] == false)
                        str_result += "1KB";
                    if (IO[1] == false && IO[0] == true)
                        str_result += "2KB";
                    if (IO[1] == true && IO[0] == false)
                        str_result += "4KB";
                    if (IO[1] == true && IO[0] == true)
                        str_result += "8KB";
                    str_result += Environment.NewLine;


                    str_result += " Block Size (w/o redundant area ) = ";
                    if (IO[5] == false && IO[4] == false)
                        str_result += "64KB";
                    if (IO[5] == false && IO[4] == true)
                        str_result += "128KB";
                    if (IO[5] == true && IO[4] == false)
                        str_result += "256KB";
                    if (IO[5] == true && IO[4] == true)
                        str_result += "512KB";
                    str_result += Environment.NewLine;


                    str_result += " Redundant Area Size ( byte/512byte) = ";
                    if (IO[2] == false)
                        str_result += "8";
                    if (IO[2] == true)
                        str_result += "16";
                    str_result += Environment.NewLine;


                    str_result += " Organization = ";
                    if (IO[6] == false)
                        str_result += "x8";
                    if (IO[6] == true)
                        str_result += "x16";
                    str_result += Environment.NewLine;

                    str_result += " Serial Access Minimum = ";
                    if (IO[7] == false && IO[3] == false)
                        str_result += "50ns/30ns";
                    if (IO[7] == true && IO[3] == false)
                        str_result += "25ns";
                    if (IO[7] == false && IO[3] == true)
                        str_result += "Reserved";
                    if (IO[7] == true && IO[3] == true)
                        str_result += "Reserved";
                    str_result += Environment.NewLine;
                    break;

                case 4:

                    str_result += " Plane Number = ";
                    if (IO[3] == false && IO[2] == false)
                        str_result += "1";
                    if (IO[3] == false && IO[2] == true)
                        str_result += "2";
                    if (IO[3] == true && IO[2] == false)
                        str_result += "4";
                    if (IO[3] == true && IO[2] == true)
                        str_result += "8";
                    str_result += Environment.NewLine;


                    str_result += " Plane Size (w/o redundant area ) = ";
                    if (IO[6] == false && IO[5] == false && IO[4] == false)
                        str_result += "64Mb";
                    if (IO[6] == false && IO[5] == false && IO[4] == true)
                        str_result += "128Mb";
                    if (IO[6] == false && IO[5] == true && IO[4] == false)
                        str_result += "256Mb";
                    if (IO[6] == false && IO[5] == true && IO[4] == true)
                        str_result += "512Mb";
                    if (IO[6] == true && IO[5] == false && IO[4] == false)
                        str_result += "1Gb";
                    if (IO[6] == true && IO[5] == false && IO[4] == true)
                        str_result += "2Gb";
                    if (IO[6] == true && IO[5] == true && IO[4] == false)
                        str_result += "4Gb";
                    if (IO[6] == true && IO[5] == true && IO[4] == true)
                        str_result += "8Gb";
                    str_result += Environment.NewLine;


                    break;
            }
            return str_result;
        }

       
    }

}
