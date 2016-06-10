using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using CrazyNateSharpDisasm;
using CrazyNateSharpDisasm.Translators;

namespace CrazyNateManaged
{
  public static class OpCodeReader
  {
    public static List<Instruction> Decompile(byte[] opCodeBytes)
    {
      ArchitectureMode architecture =
        IntPtr.Size == 4
        ? ArchitectureMode.x86_32
        : ArchitectureMode.x86_64;
      
      // Create the disassembler
      var disasm = new Disassembler(opCodeBytes, architecture, 0, true);

      // Disassemble each instruction and return in a list
      List<Instruction> results = new List<Instruction>();
      foreach (Instruction insn in disasm.Disassemble())
      {
        results.Add(insn);
      }

      return results;
    }

    public static List<PrintableFields> ToPrintableFields(this List<Instruction> instructions)
    {
      IntelTranslator translator = new IntelTranslator()
      {
        IncludeBinary = true,
        IncludeAddress = true,
      };

      List<PrintableFields> fieldsToPrint = instructions.Select(x => new PrintableFields
      {
        Address = translator.TranslateAddress(x),
        Bytes = translator.TranslateBytes(x),
        Mnemonic = translator.TranslateMnemonic(x)
      }).ToList();

      return fieldsToPrint;
    }

    [DataContract]
    public struct PrintableFields
    {
      [DataMember]
      public string Address;
      
      [DataMember]
      public string Bytes;

      [DataMember]
      public string Mnemonic;
    }

    public static List<string> ToPrintableStrings(this List<Instruction> instructions)
    {
      List<PrintableFields> fieldsToPrint = instructions.ToPrintableFields();

      int[] maxLengths = new int[3];

      foreach (PrintableFields fields in fieldsToPrint)
      {
        if (fields.Address.Length > maxLengths[0])
        {
          maxLengths[0] = fields.Address.Length;
        }

        if (fields.Bytes.Length > maxLengths[1])
        {
          maxLengths[1] = fields.Bytes.Length;
        }

        if (fields.Mnemonic.Length > maxLengths[2])
        {
          maxLengths[2] = fields.Mnemonic.Length;
        }
      }

      return fieldsToPrint.Select(x =>
        "Address: " + x.Address + ", " + string.Empty.PadLeft(maxLengths[0] - x.Address.Length) +
        "Bytes: " + x.Bytes + ", " + string.Empty.PadLeft(maxLengths[1] - x.Bytes.Length) +
        "Mnemonic: " + x.Mnemonic + string.Empty.PadLeft(maxLengths[2] - x.Mnemonic.Length)
        ).ToList();
    }
  }
}
