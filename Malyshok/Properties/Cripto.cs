using System;
using System.Security.Cryptography;
using System.Text;

    /// <summary>
    /// Класс Cripto инкапсулирует пароль пользователя.
    /// </summary>
public sealed class Cripto
{
    #region Constructor
    /// <summary>
    /// Инициализирует объект на основе указанных синхропосылки и
    /// хэша пароля.
    /// </summary>
    /// <param name="salt">Синхропосылка.</param>
    /// <param name="hash">Хэш пароля.</param>
    public Cripto(byte[] salt, byte[] hash)
    {
        _salt = (byte[])salt.Clone();
        _hash = (byte[])hash.Clone();
    }

    /// <summary>
    /// Инициализирует объект на основе указанных синхропосылки и
    /// хэша пароля
    /// </summary>
    /// <param name="salt">Base64-закодированная синхропосылка.</param>
    /// <param name="hash">Base64-закодированный хэш.</param>
    public Cripto(string salt, string hash)
    {
        _salt = Convert.FromBase64String(salt);
        _hash = Convert.FromBase64String(hash);
    }

    /// <summary>
    /// Инициализирует объект на основе указанного пароля в открытом
    /// виде.
    /// </summary>
    /// <param name="clearText">Пароль в открытом виде.</param>
    /// <remarks>По соображениям безопасности, вызывающий должен как 
    /// можно скорее обнулить этот массив.</remarks>
    public Cripto(char[] clearText)
    {
        _salt = GenerateRandom(6);
        _hash = HashPassword(clearText);
    }
    #endregion

    #region Salt
    /// <summary>
    /// Возвращает синхропосылку в виде base64-закодированной строки.
    /// </summary>
    public string Salt
    {
        get { return Convert.ToBase64String(_salt); }
    }
    #endregion

    #region RawSalt
    /// <summary>
    /// Возворащает синхропосылку в виде массива байтов.
    /// </summary>
    public byte[] RawSalt
    {
        get { return (byte[])_salt.Clone(); }
    }
    #endregion

    #region Hash
    /// <summary>
    /// Возвращает хэш пароля в виде base64-закодированной строки.
    /// </summary>
    public string Hash
    {
        get { return Convert.ToBase64String(_hash); }
    }
    #endregion

    #region RawHash
    /// <summary>
    /// Возвращает хэш пароля в виде массива байтов.
    /// </summary>
    public byte[] RawHash
    {
        get { return (byte[])_hash.Clone(); }
    }
    #endregion

    #region Verify
    /// <summary>
    /// Проверяет, что указанный пароль в открытом виде соответствует
    /// синхропосылке и хэшу, сохраненным в объекте.
    /// </summary>
    /// <param name="clearText">Пароль в открытом виде.</param>
    /// <returns>Метод возвращает true, если пароль соответствует
    /// синхропосылке и хэшу, и false - в противном случае.</returns>
    public bool Verify(char[] clearText)
    {
        byte[] hash = HashPassword(clearText);

        if (hash.Length == _hash.Length)
        {
            for (int i = 0; i < hash.Length; i++)
            {
                if (hash[i] != _hash[i])
                    return false;
            }

            return true;
        }

        return false;
    }
    #endregion

    #region Generate
    /// <summary>
    /// Генерирует случайный пароль.
    /// </summary>
    /// <returns>Сгенерированный пароль в виде массива символов.
    /// </returns>
    public static char[] Generate()
    {
        char[] random = new char[12];

        // генерируем 9 случайных байтов; этого достаточно, чтобы
        // получить 12 случайных символов из набора base64
        byte[] rnd = GenerateRandom(9);

        // конвертируем случайные байты в base64
        Convert.ToBase64CharArray(rnd, 0, rnd.Length, random, 0);

        // очищаем рабочий массив
        Array.Clear(rnd, 0, rnd.Length);

        return random;
    }
    #endregion

    #region GenerateRandom
    /// <summary>
    /// Создает массив случайных байтов указанной длины.
    /// </summary>
    /// <param name="size">Требуемая длина массива.</param>
    /// <returns>Массив случайных байтов.</returns>
    private static byte[] GenerateRandom(int size)
    {
        byte[] random = new byte[size];
        RandomNumberGenerator.Create().GetBytes(random);
        return random;
    }
    #endregion

    #region HashPassword
    /// <summary>
    /// Хэширует указанный пароль в открытом в виде в комбинации с
    /// синхропосылкой, находящейся в поле _salt.
    /// </summary>
    /// <param name="clearText">Пароль в открытом виде.</param>
    /// <returns>Хэш пароля.</returns>
    private byte[] HashPassword(char[] clearText)
    {
        Encoding utf8 = Encoding.UTF8;
        byte[] hash;

        // создаем рабочий массив достаточного размера, чтобы вместить
        byte[] data = new byte[_salt.Length
            + utf8.GetMaxByteCount(clearText.Length)];

        try
        {
            // копируем синхропосылку в рабочий массив
            Array.Copy(_salt, 0, data, 0, _salt.Length);

            // копируем пароль в рабочий массив, преобразуя его в UTF-8
            int byteCount = utf8.GetBytes(clearText, 0, clearText.Length,
                data, _salt.Length);

            // хэшируем данные массива
            using (HashAlgorithm alg = new SHA256Managed())
                hash = alg.ComputeHash(data, 0, _salt.Length + byteCount);
        }
        finally
        {
            // очищаем рабочий массив в конце работы, чтобы избежать
            // утечки открытого пароля
            Array.Clear(data, 0, data.Length);
        }

        return hash;
    }
    #endregion

    #region	Private	data members

    private byte[] _salt;		// синхропосылка
    private byte[] _hash;		// хэш пароля

    #endregion
}