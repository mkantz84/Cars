using System;
using System.ComponentModel.DataAnnotations;

namespace Cars.Code
{
    public class ValidationUserId : ValidationAttribute
    {
        public ValidationUserId()
        {
            ErrorMessage = "Id not valid";
        }

        public override bool IsValid(object value)
        {
            if (value==null)
            {
                return false;
            }
            string strID = value.ToString();
            int[] id_12_digits = { 1, 2, 1, 2, 1, 2, 1, 2, 1 };
            int count = 0;

            if (strID == null)
                return false;

            strID = strID.PadLeft(9, '0');

            for (int i = 0; i < 9; i++)
            {
                int num = Int32.Parse(strID.Substring(i, 1)) * id_12_digits[i];

                if (num > 9)
                    num = (num / 10) + (num % 10);

                count += num;
            }

            return (count % 10 == 0);
        }
    }
}