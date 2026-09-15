namespace GraphRag.Ontology.Itsm.Linguistic;

public enum TermKind
{
    /// <summary>
    /// Preferred Label.
    /// </summary>
    PreferredLabel,
    /// <summary>
    /// Synonym.
    /// Exemple : "Car" and "Automobile" are synonyms.
    /// </summary>
    Synonym,
    /// <summary>
    /// Abbreviation.
    /// Exemple : "Dr." is an abbreviation for "Doctor".
    /// </summary>
    Abbreviation,
    /// <summary>
    /// Acronym 
    /// An acronym is an abbreviation formed using the initial letters of a multi-word name or phrase. 
    /// Acronyms are often spelled with the initial letter of each word in all caps with no punctuation.
    /// Exemple : NASA, NATO, etc.
    /// </summary>
    Acronym,
    /// <summary>
    /// Hyponym.
    /// Hypernymy and hyponymy are the semantic relations between a generic term (hypernym) and a more specific term (hyponym).
    /// The hyponym names a subtype of the hypernym. 
    /// Exemple : "Car", "Truck", and "Bicycle" are hyponyms of "Vehicle".
    /// </summary>
    Hyponym,
    /// <summary>
    /// Hypernym.
    /// Hypernymy and hyponymy are the semantic relations between a generic term (hypernym) and a more specific term (hyponym).
    /// The hypernym is also called a supertype, umbrella term, or blanket term.
    /// Exemple : "Vehicle" is a hypernym of "Car", "Truck", and "Bicycle".
    /// </summary>
    Hypernym,
    /// <summary>
    /// Misspelling.
    /// Exemple : "Recieve" is a misspelling of "Receive".
    /// </summary>
    Misspelling,
}
