A wh-word (also called an interrogative or question word) is one of the English words 
that typically start a question and that most begin with the letters "wh":
•	who / whom / whose — asks about a person
•	what — asks about a thing or definition
•	which — asks to pick from options
•	when — asks about time
•	where — asks about a place
•	why — asks about a reason
•	how (and phrases like how many, how much, how does) — asks about manner / quantity / mechanism

In NerQueryAnalyzer I use it as a strong intent signal, because the wh-word usually tells you what the user wants:
Wh-word / phrase			Likely QueryIntent
how many, how much, count	Aggregation
why, how does, how do		Explanation
what, who, which + entity	EntityLookup

That's why the code has:

private static readonly HashSet<string> AggregationWh = { "how many", "how much", "count" };
private static readonly HashSet<string> ExplanationWh = { "why", "how", "how does", "how do" };
private static readonly HashSet<string> LookupWh      = { "what", "who", "which" };


The InterrogativeLemma field on NerAnalysis is where a real NER pipeline (spaCy, Azure Language, HuggingFace) 
would report the detected wh-word after lemmatization (so "what's" → "what", "how many" → "how many").

Note: French has equivalents (qui, que, quoi, quel, quand, où, pourquoi, comment) — for multilingual use 
you'd extend those sets or route via MultilingualQueryAnalyzer.
