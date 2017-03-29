using System.Collections.Generic;
using UnityEngine;

public class NameGen {
	private Dictionary<string, string[]> dict = new Dictionary<string, string[]>();
	private List<string> generated = new List<string>();
	public NameGen(string[] rules) {
		foreach (string s in rules) {
			string[] keyVal = s.Split(':');
			dict.Add(keyVal[0], keyVal[1].Split('|'));
		}
	}
	public string Generate(string nonTerminal) {
		string s = GenHelper(nonTerminal);
		for (int i = 0; generated.Contains(s) && i < 9; i++) {
			s = GenHelper(nonTerminal);
		}
		generated.Add(s);
		return s;
	}
	private string GenHelper(string nonTerminal) {
		string rule = dict[nonTerminal][Random.Range(0,dict[nonTerminal].Length)];
		foreach (string nonTerm in dict.Keys) {
			while (rule.Contains(nonTerm)) {
				rule = ReplaceFirst(rule, nonTerm, Generate(nonTerm));
			}
		}
		return rule;
	}
	public string ReplaceFirst(string text, string search, string replace) {
		int pos = text.IndexOf(search);
		if (pos < 0)
			return text;
		return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
	}


	private static NameGen townNameGen = new NameGen(new string[] {
		"<name>:<prefix><suffix>|<firstword> <lastword>",
		"<prefix>:Plain|Stag|High|Dirt|Dread|Stark|Yellow|Red|Dead|Long|Dark|Black|Deer|Maple|Steel|Dry|Glum|Grim|Crook|Ragged|White|Cattle|Low|Squirrel|Mud",
		"<suffix>:brook|vale|cliff|ridge|canyon|port|crag|worth|reach|range|stone|water|river|bend|hollow|hill",
		"<firstword>:Old|Windy|Silent|Lawless|Death's|Hangman's|Wolf's|Grizzly|Scorpion's|Yellow|Red|Black|Dead|Glum|Depressing|Grim|Desert|Dead Man's|Meme",
		"<lastword>:Landing|Canyon|Ridge|Ranch|Hollow|Stead|Stream|Spring|Gorge|Gulch|Post|Outpost|Corral"
	});
	public static string TownName() {
		return townNameGen.Generate("<name>");
	}

	private static NameGen maleNameGen = new NameGen(new string[] {
		"<name>:<first> <last>",
		"<first>:John|William|James|George|Charles|Robert|Joseph|Frank|Edward|Thomas|Henry|Walter|Harry|Willie|Arthur|Albert|Clarence|Fred|Harold|Paul|Raymond|Richard|Roy|Joe|Louis|Carl|Ralph|Earl|Jack|Ernest|David|Samuel|Howard|Charlie|Francis|Herbert|Lawrence|Theodore|Alfred|Andrew|Sam|Elmer|Eugene|Leo|Michael|Lee|Herman|Anthony|Daniel|Leonard|Floyd|Donald|Kenneth|Jesse|Russell|Clyde|Oscar|Peter|Lester|Leroy|Ray|Stanley|Clifford|Lewis|Benjamin|Edwin|Frederick|Chester|Claude|Eddie|Cecil|Lloyd|Jessie|Martin|Bernard|Tom|Will|Norman|Edgar|Harvey|Ben|Homer|Luther|Leon|Melvin|Philip|Johnnie|Jim|Milton|Everett|Allen|Leslie|Alvin|Victor|Marvin|Stephen|Alexander|Jacob|Hugh|Patrick|Virgil|Horace|Glenn|Oliver|Morris|Vernon|Archie|Julius|Gerald|Sidney|Maurice|Marion|Otis|Vincent|Guy|Earnest|Wilbur|Gilbert|Willard|Ed|Roosevelt|Hubert|Manuel|Warren|Otto|Alex|Ira|Wesley|Curtis|Wallace|Lonnie|Gordon|Jerry|Issac|Charley|Jose|Nathan|Max|Mack|Rufus|Arnold|Irving|Percy|Bill|Dan|Willis|Bernie|Jimmie|Orville|Sylvester|Rudolph|Glen|Nicholas|Dewey|Emil|Roland|Steve|Calvin|Mike|Johnie|Bert|August|Franklin|Clifton|Matthew|Emmett|Phillip|Wayne|Edmund|Abraham|Nathaniel|Dave|Marshall|Elbert|Clinton|Felix|Alton|Ellis|Nelson|Amos|Clayton|Aaron|Perry|Tony|Adam|Irvin|Dennis|Jake|Mark|Jerome|Cornelius|Douglas|Ollie|Pete|Ted|Adolph|Roger|Jay|Roscoe|Juan|Forrest|Jess|Ervin|Antonio|Gus|Owen|Moses|Bruce|Sherman|Ivan|Reuben|Don|Johnny|Claud|Booker|Alonzo|Ross|Tommie|Julian|Karl|Simon|Laurence|Wilfred|Leland|Wilson|Grady|Preston|Elijah|Wilbert|Monroe|Austin|Jasper|Harley|Marv|Bob|Delbert|Dale|Lyle|Carroll|Levi|Merle|Millard|Timothy|Loyd|Larry|Grant|Aubrey|Louie|Grover|Noah",
		
	});


	private static NameGen femaleNameGen = new NameGen(new string[] {
		"<name>:<first> <last>",
		"<first>:Mary|Helen|Margaret|Anna|Ruth|Elizabeth|Dorothy|Marie|Florence|Mildred|Alice|Ethel|Lillian|Gladys|Edna|Frances|Rose|Annie|Grace|Bertha|Emma|Bessie|Clara|Hazel|Irene|Gertrude|Louise|Catherine|Martha|Martha|Pearl|Edith|Esther|Minnie|Myrtle|Ida|Josephine|Evelyn|Elsie|Eva|Thelma|Ruby|Agnes|Sarah|Viola|Nellie|Beatrice|Julia|Laura|Lillie|Lucille|Ella|Virginia|Mattie|Pauline|Carrie|Alma|Jessie|Mae|Lena|Willie|Katherine|Blanche|Hattie|Marion|Lucy|Stella|Mamie|Vera|Cora|Fannie|Eleanor|Bernice|Jennie|Ann|Leona|Beulah|Lula|Rosa|Ada|Ellen|Kathryn|Maggie|Doris|Dora|Betty|Marguerite|Violet|Lois|Daisy|Anne|Sadie|Susie|Nora|Georgia|Maude|Marjorie|Opal|Hilda|Velma|Emily|Theresa|Charlotte|Inez|Olive|Flora|Della|Lola|Jean|Effie|Nancy|Nettiey|Sylvia|May|Lottie|Alberta|Eunice|Katie|Sallie|Genevieve|Estelle|Lydia|Loretta|Mable|Goldie|Eula|Rosie|Lizzie|Vivian|Verna|Ollie|Harriet|Addie|Lucile|Marian|Henrietta|Jane|Lela|Essie|Caroline|Ora|Iva|Sara|Maria|Madeline|Rebecca|Wilma|Barbara|Etta|Rachel|Kathleen|Irma|Christine|Geneva|Juanita|Sophie|Nina|Naomi|Victoria|Amelia|Erma|Mollie|Susan|Flossie|Ola|Nannie|Norma|Sally|Olga|Alta|Estella|Celia|Freda|Isabel|Amanda|Frieda|Luella|Matilda|Janie|Fern|Cecelia|Audrey|Winifred|Elva|Ina|Adeline|Leola|Hannah|Geraldine|Amy|Allie|Miriam|Isabelle|Bonnie|Vergie|Sophia|Jeanette|Cleo|Nell|Eliza|Selma|Roberta|Lila|Jewell|Cecilia|Veronica|Muriel|Regina|Faye|Winnie|Callie|Anita|Josie|Rena|Jeannette|Margie|Belle|Fay|Jewel|Kate|Phyllis|Augusta|Carolyn|Rita|Millie|Antoinette|Gussie|Elma|Dollie|Teresa|Adele|Claire|Tillie|Maud|Bertie|Zelma|Johnnie|Dorothea|Sue|Marcella|Leah|Letha|Roxie|Shirley|Angelina|Madge|Hester|Lorene|Elnora|Cecile",
		
	});
}