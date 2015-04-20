using System;
using System.Linq;
using System.Security.Cryptography;

namespace totp_generator
{
    class TOTP
    {
        private String _key;
        private uint _code;
        private Int32 _timeLeft;
        private readonly int _timer;

        public TOTP()
        {
            _timer = 30;
        }

        public TOTP(int timer)
        {
            _timer = timer;
        }
        public String Key
        {
            get { return _key; }
            set
            {
                _key = value.Replace(" ", "").ToUpper();
                try
                {
                    DoAlgo(Base32.ToBytes(_key));
                }
                catch (Exception e)
                {
                    _code = 0;
                }
            }
        }
        public String Code
        {
            get
            {
                // 6 because its the size of the google totp
                string res = "";
                if (_code.ToString().Length < 6)
                    for (int i = _code.ToString().Length; i < 6; i++)
                        res += "0";
                res += _code.ToString();
                return res;
            }
        }
        public String TimeLeft
        {
            get { return _timeLeft.ToString(); }
        }

        public void Refresh()
        {
            try
            {
                DoAlgo(Base32.ToBytes(_key));
            }
            catch (Exception e)
            {
                _code = 0;
            }
        }

        private void DoAlgo(byte[] keyBytes)
        {
            /*
             * Get Time
             */
            Int32 unixTime = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            _timeLeft = _timer - unixTime % _timer;
            byte[] unixBytes = BitConverter.GetBytes(unixTime / _timer);
            byte[] time = FillZeroByteArray(unixBytes);
            Array.Reverse(time);
            /*
             * Set hash
             */
            HMACSHA1 hmacsha1 = new HMACSHA1(keyBytes);
            byte[] hash = hmacsha1.ComputeHash(time);
            /*
             * Set offset
             */
            int offset = hash[hash.Length - 1] & 0x0f;
            /*
             * Set 4 byte from hash at offset
             */
            byte[] hashPart = new byte[4];
            int i = 0;
            for (int j = offset; j < offset + 4; ++j)
            {
                hashPart[i] = hash[j];
                ++i;
            }
            /*
             * Ignore the insignifiant bit
             */
            hashPart[0] = (byte)(hashPart[0] & 0x7f);
            /*
             * Convert to int
             */
            Array.Reverse(hashPart);
            uint number = BitConverter.ToUInt32(hashPart, 0);
            /*
             * Troncate 6 characters (6 because its the size of the google totp)
             */
            _code = number % 1000000;
        }

        private byte[] FillZeroByteArray(byte[] array)
        {
            int length = 8 - array.Length;
            if (length > 0)
            {
                byte[] newArray = new byte[array.Length + length];
                for (int i = 0; i < 8; ++i) newArray[i] = 0;
                array.CopyTo(newArray, 0);
                return newArray;
            }
            return array;
        }
    }
}
