namespace MinimalDB.Exceptions;
public class DuplicateKeyException(string message) : Exception(message)
{
}