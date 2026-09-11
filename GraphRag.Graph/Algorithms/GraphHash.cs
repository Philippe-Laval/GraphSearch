using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace GraphRag.Graph.Algorithms;


/// <summary>
/// Je conseille de ne jamais utiliser la date pour savoir si un embedding est périmé. On utilise un hash.
///
/// Si rien n'a changé, le hash est identique.
/// Donc on ne recalcule pas l'embedding. 
/// C'est un énorme gain de performances.
/// </summary>
public static class GraphHash
{
    /// <summary>
    /// Compute a SHA256 hash of the given YAML string and return it as a hexadecimal string.
    /// </summary>
    /// <param name="yaml"></param>
    /// <returns></returns>
    public static string Compute(string yaml)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(yaml));
        return Convert.ToHexString(bytes);
    }
}
