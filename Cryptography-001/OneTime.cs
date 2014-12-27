using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPOJ
{
    class OneTime
    {
        readonly static string[] ciphertexts = new string[] {
            "BB3A65F6F0034FA957F6A767699CE7FABA855AFB4F2B520AEAD612944A801E", 
            "BA7F24F2A35357A05CB8A16762C5A6AAAC924AE6447F0608A3D11388569A1E", 
            "A67261BBB30651BA5CF6BA297ED0E7B4E9894AA95E300247F0C0028F409A1E", 
            "A57261F5F0004BA74CF4AA2979D9A6B7AC854DA95E305203EC8515954C9D0F", 
            "BB3A70F3B91D48E84DF0AB702ECFEEB5BC8C5DA94C301E0BECD241954C831E", 
            "A6726DE8F01A50E849EDBC6C7C9CF2B2A88E19FD423E0647ECCB04DD4C9D1E", 
            "BC7570BBBF1D46E85AF9AA6C7A9CEFA9E9825CFD5E3A0047F7CD009305A71E"};
        static void __Main()
        {
            IEnumerable<int> iter = Enumerable.Range(0, ciphertexts[0].Length / 2).Select(x => x);
            List<string> ciphertexts2 = ciphertexts.Select(
                                x => new string(iter.Select(i => (char)int.Parse(x.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber)).ToArray())).ToList();
            List<List<char[]>> guess1 = new List<List<char[]>>();
            foreach (var s1 in ciphertexts2)
            {
                List<char[]> guess1_for_s1 = new List<char[]>();
                foreach (var s2 in ciphertexts2)
                {
                    if (!s1.Equals(s2))
                    {
                        guess1_for_s1.Add(s1.Select((c, i) => (char)(s2[i] ^ c))
                            .Select((xor, i) => (xor >= 0x40 && xor <= 0x80) ? (char)(xor ^ 0x20) : ' ')
                            .ToArray());
                    }
                }
                guess1.Add(guess1_for_s1);
            }
            Console.WriteLine(String.Join("", guess1.Select((c, i) =>
                            string.Format("String {0}: \n {2}\n{1}\n***********************\n", i, 
                                String.Join("", c.Select((x, index) => (index>=i ? index + 1: index).ToString() + new string(x) + '\n'))
                            , String.Join("", Enumerable.Range(0, 30).Select(x => (x % 10).ToString()))))));
            Console.ReadKey();   
        }
    }
}
