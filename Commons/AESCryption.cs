using System.Security.Cryptography;
using System.Text;

namespace semigura.Commons
{
    public static class AESCryption
    {
        //※参考：https://qiita.com/kz-rv04/items/62a56bd4cd149e36ca70
        //AES暗号とは？
        //AES(Advanced Encryption Standard)はRijndaelとも呼ばれ
        //旧規格の対称鍵暗号であるDES(Data Encryption Standard)の安全性が低下したために、
        //NIST(アメリカ国立標準技術研空所)が公募し、2000年に選定された対称暗号である。

        private const string AES_IV = @"pf69DL6GrWFyZcMK";
        private const string AES_Key = @"9Fix4L4HB4PKeKWY";

        public static void Main()
        {
            // 平文の文字列
            string plainText = "Hello, World!";

            Console.WriteLine("PlainText : {0}\n", plainText);

            // 暗号化された文字列
            string cipher = Encrypt(plainText, AES_IV, AES_Key);
            Console.WriteLine("Cipher : {0}\n", cipher);

            Console.WriteLine("Decrypted : {0}\n", Decrypt(cipher, AES_IV, AES_Key));


            //PlainText : Hello, World!

            //Cipher : p8ITppfvm6QnVtL/Ji9/ZQ==

            //Decrypted : Hello, World!
        }

        /// <summary>
        /// 対称鍵暗号を使って文字列を暗号化する
        /// </summary>
        /// <param name="text">暗号化する文字列</param>
        /// <param name="iv">対称アルゴリズムの初期ベクター</param>
        /// <param name="key">対称アルゴリズムの共有鍵</param>
        /// <returns>暗号化された文字列</returns>
        public static string Encrypt(string text, string iv, string key)
        {

            using (RijndaelManaged rijndael = new RijndaelManaged())
            {
                rijndael.BlockSize = 128;
                rijndael.KeySize = 128;
                rijndael.Mode = CipherMode.CBC;
                rijndael.Padding = PaddingMode.PKCS7;

                rijndael.IV = Encoding.UTF8.GetBytes(iv);
                rijndael.Key = Encoding.UTF8.GetBytes(key);

                ICryptoTransform encryptor = rijndael.CreateEncryptor(rijndael.Key, rijndael.IV);

                byte[] encrypted;
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream ctStream = new CryptoStream(mStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(ctStream))
                        {
                            sw.Write(text);
                        }
                        encrypted = mStream.ToArray();
                    }
                }
                return (System.Convert.ToBase64String(encrypted));
            }
        }

        /// <summary>
        /// 対称鍵暗号を使って暗号文を復号する
        /// </summary>
        /// <param name="cipher">暗号化された文字列</param>
        /// <param name="iv">対称アルゴリズムの初期ベクター</param>
        /// <param name="key">対称アルゴリズムの共有鍵</param>
        /// <returns>復号された文字列</returns>
        public static string Decrypt(string cipher, string iv, string key)
        {
            using (RijndaelManaged rijndael = new RijndaelManaged())
            {
                rijndael.BlockSize = 128;
                rijndael.KeySize = 128;
                rijndael.Mode = CipherMode.CBC;
                rijndael.Padding = PaddingMode.PKCS7;

                rijndael.IV = Encoding.UTF8.GetBytes(iv);
                rijndael.Key = Encoding.UTF8.GetBytes(key);

                ICryptoTransform decryptor = rijndael.CreateDecryptor(rijndael.Key, rijndael.IV);

                string plain = string.Empty;
                using (MemoryStream mStream = new MemoryStream(System.Convert.FromBase64String(cipher)))
                {
                    using (CryptoStream ctStream = new CryptoStream(mStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(ctStream))
                        {
                            plain = sr.ReadLine();
                        }
                    }
                }
                return plain;
            }
        }
    }
}