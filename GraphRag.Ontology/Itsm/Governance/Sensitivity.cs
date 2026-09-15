namespace GraphRag.Ontology.Itsm.Governance;

/// <summary>
/// Représente les niveaux de sensibilité des données, y compris les catégories réglementées d’informations
/// personnelles, médicales et de paiement.
/// </summary>
/// <remarks>Utiliser cette énumération pour appliquer des règles de classification, d’accès, de stockage et de
/// conformité selon le niveau de sensibilité.</remarks>
public enum Sensitivity
{ 
    /// <summary>
    /// Public
    /// </summary>
    Public, 
    
    /// <summary>
    /// Internal
    /// </summary>
    Internal, 
    
    /// <summary>
    /// Confidential
    /// </summary>
    Confidential,
    
    /// <summary>
    /// Restricted
    /// </summary>
    Restricted,
    
    /// <summary>
    /// PII (Personally Identifiable Information)
    /// </summary>
    Pii,
    
    /// <summary>
    /// PHI (Protected Health Information)
    /// </summary>
    Phi,
    
    /// <summary>
    /// PCI (Payment Card Industry)
    /// </summary>
    Pci
}

/*
Les PII, PHI et PCI désignent trois catégories de données sensibles nécessitant 
des mesures de sécurité et de conformité informatique spécifiques. 


PII (Personally Identifiable Information)
Définition : Informations personnelles identifiables qui permettent d'identifier directement 
ou indirectement une personne physique.
Exemples : Nom complet, adresse e-mail, numéro de téléphone, date de naissance, numéro de sécurité sociale.
Enjeu : Il s'agit du type global de données personnelles qui sert souvent de point d'entrée pour l'usurpation d'identité. 


PHI (Protected Health Information)
Définition : Informations médicales protégées, qui constituent un sous-ensemble des PII.
Exemples : Dossiers médicaux, résultats d'examens, historique de soins, données de facturation liée à la santé.
Enjeu : Ces données sont strictement réglementées (notamment par la loi HIPAA aux États-Unis ou le RGPD en Europe) 
dès qu'elles sont rattachées à un identifiant patient. 


PCI (Payment Card Industry)
Définition : Données relatives aux cartes de paiement et aux transactions financières.
Exemples : Numéro de carte bancaire (PAN), nom du titulaire, code PIN, cryptogramme.
Enjeu : Les entreprises qui manipulent ces données doivent se soumettre à la norme de sécurité PCI DSS 
pour prévenir la fraude financière. 
 */