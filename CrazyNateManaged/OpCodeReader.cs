using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CrazyNateSharpDisasm;

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

      // Configure the translator to output instruction addresses and instruction binary as hex
      Disassembler.Translator.IncludeAddress = true;
      Disassembler.Translator.IncludeBinary = true;
      
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
  }
}
