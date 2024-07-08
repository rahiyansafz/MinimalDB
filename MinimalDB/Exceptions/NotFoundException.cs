namespace MinimalDB.Exceptions;
public class NotFoundException(string message) : Exception(message)
{
}