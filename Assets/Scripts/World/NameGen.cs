using System.Collections.Generic;
using UnityEngine;

public class NameGen {
	private Dictionary<string, string[]> dict = new Dictionary<string, string[]>();
	private List<string> generated = new List<string>();
	public NameGen(params string[] rules) {
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

	private static NameGen townName = new NameGen(
		"<name>:<prefix><suffix>|<firstword> <lastword>",
		"<prefix>:Plain|Stag|High|Dirt|Dread|Stark|Yellow|Red|Dead|Long|Dark|Black|Deer|Maple|Steel|Dry|Glum|Grim|Crook|Ragged|White|Cattle|Low|Squirrel|Mud",
		"<suffix>:brook|vale|cliff|ridge|canyon|port|crag|worth|reach|range|stone|water|river|bend|hollow|hill|run|hole|burrow|wood|valley",
		"<firstword>:Old|Windy|Silent|Lawless|Death's|Hangman's|Wolf's|Grizzly|Scorpion's|Yellow|Red|Black|Dead|Glum|Depressing|Grim|Desert|Dead Man's|Meme",
		"<lastword>:Landing|Canyon|Ridge|Ranch|Hollow|Stead|Stream|Spring|Gorge|Gulch|Post|Outpost|Corral|Hole|Valley"
	);
	public static string TownName() {
		return townName.Generate("<name>");
	}

	private static NameGen personNameGen = new NameGen(
		"<male-first>:Dutch|John|William|James|George|Charles|Robert|Joseph|Frank|Edward|Thomas|Henry|Walter|Harry|Willie|Arthur|Albert|Clarence|Fred|Harold|Paul|Raymond|Richard|Roy|Joe|Louis|Carl|Ralph|Earl|Jack|Ernest|David|Samuel|Howard|Charlie|Francis|Herbert|Lawrence|Theodore|Alfred|Andrew|Sam|Elmer|Eugene|Leo|Michael|Lee|Herman|Anthony|Daniel|Leonard|Floyd|Donald|Kenneth|Jesse|Russell|Clyde|Oscar|Peter|Lester|Leroy|Ray|Stanley|Clifford|Lewis|Benjamin|Edwin|Frederick|Chester|Claude|Eddie|Cecil|Lloyd|Jessie|Martin|Bernard|Tom|Will|Norman|Edgar|Harvey|Ben|Homer|Luther|Leon|Melvin|Philip|Johnnie|Jim|Milton|Everett|Allen|Leslie|Alvin|Victor|Marvin|Stephen|Alexander|Jacob|Hugh|Patrick|Virgil|Horace|Glenn|Oliver|Morris|Vernon|Archie|Julius|Gerald|Sidney|Maurice|Marion|Otis|Vincent|Guy|Earnest|Wilbur|Gilbert|Willard|Ed|Roosevelt|Hubert|Manuel|Warren|Otto|Alex|Ira|Wesley|Curtis|Wallace|Lonnie|Gordon|Jerry|Issac|Charley|Jose|Nathan|Max|Mack|Rufus|Arnold|Irving|Percy|Bill|Dan|Willis|Bernie|Jimmie|Orville|Sylvester|Rudolph|Glen|Nicholas|Dewey|Emil|Roland|Steve|Calvin|Mike|Johnie|Bert|August|Franklin|Clifton|Matthew|Emmett|Phillip|Wayne|Edmund|Abraham|Nathaniel|Dave|Marshall|Elbert|Clinton|Felix|Alton|Ellis|Nelson|Amos|Clayton|Aaron|Perry|Tony|Adam|Irvin|Dennis|Jake|Mark|Jerome|Cornelius|Douglas|Ollie|Pete|Ted|Adolph|Roger|Jay|Roscoe|Juan|Forrest|Jess|Ervin|Antonio|Gus|Owen|Moses|Bruce|Sherman|Ivan|Reuben|Don|Johnny|Claud|Booker|Alonzo|Ross|Tommie|Julian|Karl|Simon|Laurence|Wilfred|Leland|Wilson|Grady|Preston|Elijah|Wilbert|Monroe|Austin|Jasper|Harley|Marv|Bob|Delbert|Dale|Lyle|Carroll|Levi|Merle|Millard|Timothy|Loyd|Larry|Grant|Aubrey|Louie|Grover|Noah",
		"<female-first>:Mary|Helen|Margaret|Anna|Ruth|Elizabeth|Dorothy|Marie|Florence|Mildred|Alice|Ethel|Lillian|Gladys|Edna|Frances|Rose|Annie|Grace|Bertha|Emma|Bessie|Clara|Hazel|Irene|Gertrude|Louise|Catherine|Martha|Martha|Pearl|Edith|Esther|Minnie|Myrtle|Ida|Josephine|Evelyn|Elsie|Eva|Thelma|Ruby|Agnes|Sarah|Viola|Nellie|Beatrice|Julia|Laura|Lillie|Lucille|Ella|Virginia|Mattie|Pauline|Carrie|Alma|Jessie|Mae|Lena|Willie|Katherine|Blanche|Hattie|Marion|Lucy|Stella|Mamie|Vera|Cora|Fannie|Eleanor|Bernice|Jennie|Ann|Leona|Beulah|Lula|Rosa|Ada|Ellen|Kathryn|Maggie|Doris|Dora|Betty|Marguerite|Violet|Lois|Daisy|Anne|Sadie|Susie|Nora|Georgia|Maude|Marjorie|Opal|Hilda|Velma|Emily|Theresa|Charlotte|Inez|Olive|Flora|Della|Lola|Jean|Effie|Nancy|Nettiey|Sylvia|May|Lottie|Alberta|Eunice|Katie|Sallie|Genevieve|Estelle|Lydia|Loretta|Mable|Goldie|Eula|Rosie|Lizzie|Vivian|Verna|Ollie|Harriet|Addie|Lucile|Marian|Henrietta|Jane|Lela|Essie|Caroline|Ora|Iva|Sara|Maria|Madeline|Rebecca|Wilma|Barbara|Etta|Rachel|Kathleen|Irma|Christine|Geneva|Juanita|Sophie|Nina|Naomi|Victoria|Amelia|Erma|Mollie|Susan|Flossie|Ola|Nannie|Norma|Sally|Olga|Alta|Estella|Celia|Freda|Isabel|Amanda|Frieda|Luella|Matilda|Janie|Fern|Cecelia|Audrey|Winifred|Elva|Ina|Adeline|Leola|Hannah|Geraldine|Amy|Allie|Miriam|Isabelle|Bonnie|Vergie|Sophia|Jeanette|Cleo|Nell|Eliza|Selma|Roberta|Lila|Jewell|Cecilia|Veronica|Muriel|Regina|Faye|Winnie|Callie|Anita|Josie|Rena|Jeannette|Margie|Belle|Fay|Jewel|Kate|Phyllis|Augusta|Carolyn|Rita|Millie|Antoinette|Gussie|Elma|Dollie|Teresa|Adele|Claire|Tillie|Maud|Bertie|Zelma|Johnnie|Dorothea|Sue|Marcella|Leah|Letha|Roxie|Shirley|Angelina|Madge|Hester|Lorene|Elnora|Cecile",
		"<last>:Van Der Linde|Marston|Smith|Johnson|Williams|Jones|Brown|Davis|Miller|Wilson|Moore|Taylor|Anderson|Thomas|Jackson|White|Harris|Martin|Thompson|Robinson|Clark|Rodriguez|Lewis|Lee|Walker|Hall|Allen|Young|King|Wright|Lopez|Hill|Scott|Green|Adams|Baker|Gonzalez|Nelson|Carter|Mitchell|Perez|Roberts|Turner|Phillips|Campbell|Parker|Evans|Edwards|Collins|Stewart|Sanchez|Morris|Rogers|Reed|Cook|Morgan|Bell|Murphy|Bailey|Rivera|Cooper|Richardson|Cox|Howard|Ward|Torres|Peterson|Gray|Ramirez|James|Watson|Brooks|Kelly|Sanders|Price|Bennett|Wood|Barnes|Ross|Henderson|Coleman|Jenkins|Perry|Powell|Long|Patterson|Hughes|Flores|Washington|Butler|Simmons|Foster|Gonzales|Bryant|Alexander|Russell|Griffin|Diaz|Hayes|Myers|Ford|Hamilton|Graham|Sullivan|Wallace|Woods|Cole|West|Jordan|Owens|Reynolds|Fisher|Ellis|Harrison|Gibson|McDonald|Cruz|Marshall|Gomez|Murray|Freeman|Wells|Webb|Simpson|Stevens|Tucker|Porter|Hunter|Hicks|Crawford|Henry|Boyd|Mason|Morales|Kennedy|Warren|Dixon|Ramos|Reyes|Burns|Gordon|Shaw|Holmes|Rice|Robertson|Hunt|Black|Daniels|Palmer|Mills|Nichols|Grant|Knight|Ferguson|Rose|Stone|Hawkins|Dunn|Perkins|Hudson|Spencer|Gardner|Stephens|Payne|Pierce|Berry|Matthews|Arnold|Wagner|Willis|Ray|Watkins|Olson|Carroll|Duncan|Snyder|Hart|Cunningham|Bradley|Lane|Andrews|Ruiz|Harper|Fox|Riley|Armstrong|Carpenter|Weaver|Greene|Lawrence|Elliott|Sims|Austin|Peters|Kelley|Franklin|Lawson|Fields|Ryan|Schmidt|Carr|Vasquez|Castillo|Wheeler|Chapman|Oliver|Montgomery|Richards|Williamson|Johnston|Banks|Meyer|Bishop|McCoy|Howell|Alvarez|Morrison|Hansen|Harvey|Little|Burton|Stanley|Nguyen|George|Jacobs|Reid|Kim|Fuller|Lynch|Dean|Gilbert|Garrett|Romero|Welch|Larson|Frazier|Burke|Hanson|Day|Moreno|Bowman|Medina|Fowler|Brewer|Hoffman|Carlson|Silva|Pearson|Holland|Douglas|Fleming|Jensen|Vargas|Byrd|Davidson|Hopkins|May|Terry|Herrera|Wade|Soto|Walters|Curtis|Neal|Caldwell|Lowe|Jennings|Barnett|Graves|Jimenez|Horton|Shelton|Barrett|O'Brien|Castro|Sutton|Gregory|McKinney|Lucas|Miles|Craig|Rodriquez|Chambers|Holt|Lambert|Fletcher|Watts|Bates|Hale|Rhodes|Pena|Beck|Newman|Haynes|McDaniel|Mendez|Bush|Vaughn|Parks|Dawson|Norris|Hardy|Love|Steele|Curry|Powers|Schultz|Barker|Guzman|Page|Munoz|Ball|Keller|Chandler|Weber|Leonard|Walsh|Lyons|Ramsey|Wolfe|Schneider|Mullins|Benson|Sharp|Bowen|Daniel|Barber|Cummings|Hines|Baldwin|Griffith|Valdez|Hubbard|Salazar|Reeves|Warner|Stevenson|Burgess|Santos|Tate|Cross|Garner|Mann|Mack|Moss|Thornton|Dennis|McGee|Farmer|Delgado|Aguilar|Vega|Glover|Manning|Cohen|Harmon|Rodgers|Robbins|Newton|Todd|Blair|Higgins|Ingram|Reese|Cannon|Strickland|Townsend|Potter|Goodwin|Walton|Rowe|Hampton|Ortega|Patton|Swanson|Joseph|Francis|Goodman|Maldonado|Yates|Becker|Erickson|Hodges|Rios|Conner|Adkins|Webster|Norman|Malone|Hammond|Flowers|Cobb|Moody|Quinn|Blake|Maxwell|Pope|Floyd|Osborne|Paul|McCarthy|Guerrero|Lindsey|Estrada|Sandoval|Gibbs|Tyler|Gross|Fitzgerald|Stokes|Doyle|Sherman|Saunders|Wise|Colon|Gill|Alvarado|Greer|Padilla|Simon|Waters|Nunez|Ballard|Schwartz|McBride|Houston|Christensen|Klein|Pratt|Briggs|Parsons|McLaughlin|Zimmerman|French|Buchanan|Moran|Copeland|Roy|Pittman|Brady|McCormick|Holloway|Brock|Poole|Frank|Logan|Owen|Bass|Marsh|Drake|Wong|Jefferson|Park|Morton|Abbott|Sparks|Patrick|Norton|Huff|Clayton|Massey|Lloyd|Figueroa|Carson|Bowers|Roberson|Barton|Tran|Lamb|Harrington|Casey|Boone|Cortez|Clarke|Mathis|Singleton|Wilkins|Cain|Bryan|Underwood|Hogan|McKenzie|Collier|Luna|Phelps|McGuire|Allison|Bridges|Wilkerson|Nash|Summers|Atkins|Wilcox|Pitts|Conley|Burnett|Richard|Cochran|Chase|Davenport|Hood|Gates|Clay|Ayala|Sawyer|Roman|Dickerson|Hodge|Acosta|Flynn|Espinoza|Nicholson|Monroe|Wolf|Morrow|Kirk|Randall|Anthony|Whitaker|O'Connor|Skinner|Ware|Molina|Kirby|Huffman|Bradford|Charles|Gilmore|O'Neal|Bruce|Lang|Combs|Kramer|Heath|Hancock|Gallagher|Gaines|Shaffer|Short|Wiggins|Mathews|McClain|Fischer|Wall|Small|Melton|Hensley|Bond|Dyer|Cameron|Grimes|Contreras|Christian|Wyatt|Baxter|Snow|Mosley|Shepherd|Larsen|Hoover|Beasley|Glenn|Petersen|Whitehead|Meyers|Keith|Garrison|Vincent|Shields|Horn|Savage|Olsen|Schroeder|Hartman|Woodard|Mueller|Kemp|Deleon|Booth|Patel|Calhoun|Wiley|Eaton|Cline|Navarro|Harrell|Lester|Humphrey|Parrish|Duran|Hutchinson|Hess|Dorsey|Bullock|Robles|Beard|Dalton|Van Elderen|Vance|Rich|Blackwell|York|Johns|Blankenship|Salinas|Campos|Pruitt|Moses|Callahan|Golden|Montoya|Hardin|Guerra|McDowell|Carey|Stafford|Gallegos|Henson|Wilkinson|Booker|Merritt|Miranda|Atkinson|Orr|Decker|Hobbs|Preston|Tanner|Knox|Pacheco|Stephenson|Glass|Rojas|Serrano|Marks|Hickman|English|Sweeney|Strong|Prince|McClure|Conway|Walter|Roth|Maynard|Farrell|Lowery|Hurst|Nixon|Weiss|Bonnell|Ellison|Sloan|Winters|McLean|Randolph|Leon|Boyer|Villarreal|McCall|Gentry|Carrillo|Kent|Ayers|Lara|Shannon|Sexton|Pace|Hull|Leblanc|Browning|Velasquez|Leach|Chang|House|Sellers|Herring|Noble|Foley|Bartlett|Heimerl|Landry|Durham|Walls|Barr|McKee|Bauer|Rivers|Everett|Bradshaw|Walsh|Rush|Estes|Dodson|Morse|Sheppard|Weeks|Camacho|Bean|Barron|Livingston|Middleton|Spears|Branch|Blevins|Chen|Kerr|McConnell|Hatfield|Harding|Ashley|Solis|Herman|Frost|Giles|Blackburn|William|Pennington|Woodward|Finley|McIntosh|Koch|Best|Solomon|McCullough|Dudley|Nolan|Blanchard|Rivas|Brennan|Kane|Benton|Joyce|Buckley|Haley|Valentine|Maddox|Russo|McKnight|Buck|Moon|McMillan|Crosby|Berg|Dotson|Mays|Roach|Church|Chan|Richmond|Meadows|Faulkner|O'Neill|Knapp|Kline|Barry|Jacobson|Avery|Hendricks|Horne|Shepard|Hebert|Cherry|Cardenas|McIntyre|Whitney|Waller|Holman|Donaldson|Terrell|Morin|Gillespie|Fuentes|Tillman|Sanford|Bentley|Peck|Key|Rollins|Gamble|Dickson|Battle|Santana|Cabrera|Cervantes|Howe|Hinton|Hurley|Spence|Yang|McNeil|Suarez|Case|Petty|Gould|McFarland|Sampson|Carver|Bray|Rosario|MacDonald|Stout|Hester|Melendez|Dillon|Farley|Hopper|Galloway|Potts|Bernard|Joyner|Stein|Aguirre|Osborn|Mercer|Bender|Franco|Rowland|Sykes|Benjamin|Travis|Pickett|Crane|Sears|Mayo|Dunlap|Hayden|Wilder|McKay|Coffey|McCarty|Ewing|Cooley|Vaughan|Bonner|Cotton|Holder|Stark|Ferrell|Cantrell|Fulton|Lynn|Lott|Calderon|Rosa|Pollard|Hooper|Burch|Mullen|Fry|Riddle|Levy|David|Duke|O'Donnell|Guy|Michael|Britt|Frederick|Daugherty|Berger|Dillard|Alston|Jarvis|Frye|Riggs|Chaney|Odom|Duffy|Fitzpatrick|Valenzuela|Merrill|Mayer|Alford|McPherson|Reges|Donovan|Barrera|Albert|Cote|Reilly|Compton|Raymond|Mooney|McgGowan|Craft|Cleveland|Clemons|Wynn|Nielsen|Baird|Stanton|Snider|Rosales|Bright|Witt|Stuart|Hays|Holden|Rutledge|Kinney|Clements|Castaneda|Slater|Hahn|Emerson|Conrad|Burks|Delaney|Pate|Lancaster|Sweet|Justice|Tyson|Sharpe|Whitfield|Talley|Macias|Irwin|Burris|Ratliff|McCray|Madden|Kaufman|Beach|Goff|Cash|Bolton|McFadden|Levine|Good|Byers|Kirkland|Kidd|Workman|Carney|Dale|McLeod|Holcomb|England|Finch|Head|Burt|Hendrix|Sosa|Haney|Franks|Sargent|Nieves|Downs|Rasmussen|Bird|Hewitt|Lindsay|Le|Foreman|Valencia|O'Neil|Vinson|Hyde|Forbes|Gilliam|Guthrie|Wooten|Huber|Barlow|Boyle|McMahon|Buckner|Puckett|Langley|Knowles|Cooke|Whitley|Noel"
	);
	public static string CharacterFirstName(bool female = false) {
		return personNameGen.Generate(female ? "<female-first>" : "<male-first>");
	}
	public static string CharacterLastName() {
		return personNameGen.Generate("<last>");
	}	

	private static NameGen gangNameGen = new NameGen(
		"<gangName>:<name>'s <nameSuffix>|[lastName] <nameSuffix>|The <adj> <pluralNoun>|The <pluralNoun>",
		"<name>:[firstName]|[lastName]",
		"<nameSuffix>:Gang|Boys|Bunch|Crew",
		"<adj>:Dirty|Old|Wild|Sneaky|Grimy",
		"<pluralNoun>:Cowboys|Renegades|Regulators|Rustlers|Ruffians"
	);
	public static string GangName(string leaderFirstName, string leaderLastName) {
		if (Random.Range(0, 1000) < 5) {
			return "The Monocle Boys";
		}
		return gangNameGen.Generate("<gangName>")
				.Replace("[firstName]", leaderFirstName)
				.Replace("[lastName]", leaderLastName);
	}
}