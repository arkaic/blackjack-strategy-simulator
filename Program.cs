using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackSimulator {
    public static class Globals {
        public static IShoe shoe;
        public static List<Hand> handsWithBJ;
        public static int numDecks;
        public static int runs; 
        public const int ORDERED = 1;
        public const int SHUFFLED = 2;
        public const int H =  1;
        public const int S =  2;
        public const int DH = 3;
        public const int DS = 4;
        public const int P  = 5;
        public const int PH = 6;
        public const int RH = 7;
        public const int K  = 0;
        public const int NEXT_ROUND = 1;
        public const int BREAK = 2;
        public static bool doInsurance = false;
        public static bool doubleAllowedAfterSplit = true;
        public static bool surrendersAllowed = true;
        public static bool houseStaysOnSoft17 = true;
    }


    public static class Stats {
        public static decimal houseBank = 0.00m;
        public static decimal playerBank = 0.00m;
        public static int wins = 0;
        public static int losses = 0;
        public static int pushes = 0;
        public static int surrenders = 0;
        public static decimal totalBets = 0.00m;

	    //or get calculated values from somewhere
        //actual value = current (+/-)bankroll divided by total wagers made
        public static decimal ActualEdge() {            
            return playerBank / totalBets;
        }
    }


    public static class Strategy {
        public static int e;
    }


    public class Hand {
        public List<int> cards = new List<int>();
        public decimal bet = 0.00m;
        public decimal insuranceBet = 0.00m;
        public int hardVal = 0;
        public int realVal = 0;
        public bool hasAce = false;
        public bool isSplit = false;
        public bool isDoubled = false;
        public bool isDealer = false;
        public Hand() 
            : this(0.00m) {}
        public Hand(decimal bet) {
        	this.bet = bet;
        }
        public void DealCard(int card) {
            cards.Add(card);
            hardVal += card;
            if (hardVal >= 12) {
                realVal = hardVal;
            } else {
                if (!hasAce) {
                    if (card != 1) {
                        realVal = hardVal;
                    } else {
                        hasAce = true;
                        realVal = hardVal + 10;
                    }
                } else {
                    realVal = hardVal + 10;
                }
            }
        }
        public bool IsSoft() {
            bool returnVal = (realVal==hardVal) ? false : true;
            return returnVal;
        }
    }


    public interface IShoe {
        void Generate(int numDecks);
        void Shuffle();
        int Pop();
        int Count();
    }


    public class ShoeList : IShoe {
        public List<int> cards;

        public void Shuffle() {
            Random rng = new Random();
            int n = cards.Count;  
            while (n > 1) {  
                n--;  
                int k = rng.Next(n + 1);                 
                int value = cards[k];  
                cards[k] = cards[n];  
                cards[n] = value;  
            }
        }

        public void Generate(int numDecks) {
            cards = new List<int>();
            for (int i = 0; i < numDecks; i++) {
                for (int j = 0; j < 52; j++) {
                    int cardVal = j%13 + 1;
                    if (cardVal <= 10)
                        cards.Add(cardVal);
                    else
                        cards.Add(10);
                }
            }
        }

        public int Pop() {
        	int topCard = cards[cards.Count()-1];
        	cards.RemoveAt(cards.Count() - 1);
            return topCard;
        }

        public int Count() {
            return cards.Count();
        }
    }


    public class ShoeStack : IShoe {
        public void Generate(int numDecks) {
            //Implement
        }

        public void Shuffle() {
        	//Implement
        }

        public int Pop() {
            //Implement
            return 0;
        }

        public int Count() {
        	//Implement
        	return 0;
        }
    }


    public class SimulatorProgram {
        static void Testing() {
            Console.WriteLine("Blackjack Simulator!!!!!!!!");
            Console.WriteLine("houseBank = {0:C}", Stats.houseBank);
            Console.WriteLine("playerBank = {0:C}", Stats.houseBank);
            Stats.houseBank *= (decimal)1.5;
            Console.WriteLine("Stat.houseBank *= 1.5 = {0:C}", Stats.houseBank);
            Hand hand = new Hand();
            Console.WriteLine("hand.hardVal = {0}\nhand.realVal = {1}", hand.hardVal, hand.realVal);
            Console.WriteLine("issoft() = {0}", hand.IsSoft());
            Console.WriteLine("test LIST for cards...");
            hand.cards.Add(5);
            hand.cards.Add(2);
            Console.WriteLine("hand.cards = [{0}, {1}]", hand.cards[0], hand.cards[1]);
            PrintCards(hand.cards);
        }

        static void Test2() {
            // rounds up on blackjack payouts to nearest half (1.00, 1.50, 
            // 2.00, etc);
            decimal payout = Stats.playerBank * (decimal)1.5;
            Console.WriteLine("{0:C} => {1:C}", payout, Math.Ceiling(payout*2)/2);
        }

        public static void PrintCards<T>(IEnumerable<T> col) {
            foreach (T item in col)
                Console.WriteLine(item); // Replace this with your version of printing
	    }

	    public static void SetupHands(List<Hand> hands, int numPlayerHands, decimal startBet) {
	    	for (int i = 0; i < numPlayerHands; i++) {
	    		hands.Add(new Hand(startBet));
	    	}
	    	Hand dealer = new Hand();
	    	dealer.isDealer = true;
	    	hands.Add(dealer);
	    }

	    public static void DealStartingCards(List<Hand> hands) {
	    	Globals.handsWithBJ = new List<Hand>();
	    	foreach (Hand hand in hands) {
	    		hand.DealCard(Globals.shoe.Pop());
	    	}
	    	foreach (Hand hand in hands) {
	    		hand.DealCard(Globals.shoe.Pop());
	    		if (hand.realVal==21 || !hand.isDealer) {
	    			// If hand is blackjack and is not dealer
	    			Globals.handsWithBJ.Add(hand);
	    		}
	    	}
	    }

        // Take all bets except for ties
	    public static void TakeAllBets(List<Hand> hands) {	
            int numPlayerHands = hands.Count() - 1;
            for (int i = 0; i < numPlayerHands; i++) {
                if (hands[i].realVal < 21) {
                    Stats.playerBank -= hands[i].bet;
                    Stats.houseBank += hands[i].bet;
                    Stats.losses++;
                } else if (hands[i].realVal == 21) {
                    Stats.pushes++;
                }
            }
        }

        private static void PayAllInsurances(List<Hand> hands) {
            int numPlayerHands = hands.Count() - 1;
            for (int i = 0; i < numPlayerHands; i++) {
                if (hands[i].insuranceBet > 0) {
                    decimal payout = 2 * hands[i].insuranceBet;
                    Stats.houseBank -= payout;
                    Stats.playerBank += payout;
                }
            }
        }

        private static void TakeAllInsurances(List<Hand> hands) {
            int numPlayerHands = hands.Count() - 1;
            for (int i = 0; i < numPlayerHands; i++) {
                Stats.houseBank += hands[i].insuranceBet;
                Stats.playerBank -= hands[i].insuranceBet;
                hands[i].insuranceBet = 0.00m;
            }
        }

        private static void PayAllBlackjacks(List<Hand> hands) {
            for (int i = 0; i < Globals.handsWithBJ.Count(); i++) {
                // Round every blackjack payout up to the nearest half.
                Hand bjHand = Globals.handsWithBJ[i];
                decimal payout = Math.Ceiling((decimal)1.5 * bjHand.bet * 2) / 2;
                Stats.playerBank += payout;
                Stats.houseBank -= payout;
                Stats.wins++;
                hands.Remove(bjHand);
            }
        }

        public static bool CheckDealerForBlackjack(List<Hand> hands) {
            Hand dealer = hands[hands.Count()-1];
            if (dealer.cards[0]==10 && dealer.cards[1]==1) {
                TakeAllBets(hands);                
                return true;
            } else if (dealer.cards[0] == 1) {
                if (Globals.doInsurance) {
                    Console.WriteLine("Doing insurance");
                    // TODO: implement Setup insurance
                }
                if (dealer.cards[1] == 10) {
                    // Dealer has blackjack
                    TakeAllBets(hands);
                    if (Globals.doInsurance) 
                        PayAllInsurances(hands);
                    return true;
                } else {
                    // Dealer has no blackjack
                    if (Globals.doInsurance)
                        TakeAllInsurances(hands);
                    return false;
                }        
            } else {
                // Dealer is not showing a face or ace
                return false;
            }
        }

        public static void GameLoop() {
            /* 
               while len(shoe) >= shoesize/5:
                    x setup hands, wagers
                    x deal cards
                    x check for dealer bj
                    x check for dealer ace-bj and insurance
                    resolve player blackjacks
                    
                    loop over hands:
                       deal card if hand size = 1
                       check for splitting option
                       loop for dealing cards:
                           decisions, decisions
                       if hand busts or surrenders, do stuff and remove it
                       else, increment hand counter
                    
                    if there are player hands left:
                        deal to house and evaluate
                        pay and take
                    
                    clear hands
                    optional: strategy review 
            */

            int shoeSize = Globals.shoe.Count();
            Globals.shoe.Pop(); // Burn card

            //Rounds Loop
            while (Globals.shoe.Count() >= (shoeSize/5)) {
            	// Set up hands
            	List<Hand> hands = new List<Hand>();                
                int numPlayerHands = 2; // Soon to be affected by strategy
                decimal startBet = 5.00m; // Soon to be affected by strategy
                SetupHands(hands, numPlayerHands, startBet);
                Hand dealer = hands[hands.Count()-1];

                // Deal cards to every hand
                DealStartingCards(hands);

                // Check dealer for blackjack
                if (CheckDealerForBlackjack(hands)) {
                    Console.WriteLine("END ROUND: HOUSE HAS BJ");
                    continue;
                }

                // Pay player blackjacks
                if (Globals.handsWithBJ.Count() > 0) {
                    PayAllBlackjacks(hands);
                }

                break; // TODO: remove after debug
            }
        }

        public static void RunSimulation(string[] args) {
            Globals.numDecks = 6;
            Globals.shoe = new ShoeStack();
            Globals.shoe.Generate(Globals.numDecks);
            if (args.Count() == 1)
                Globals.runs = Convert.ToInt32(args[0]);
            else 
                Globals.runs = 1;
            Console.WriteLine(Globals.runs + " runs");
            // Shoe runs loop
            while (Globals.runs > 0) {
            	Globals.shoe.Shuffle();
                Console.WriteLine("cards in shoe " + Globals.shoe.Count());
                GameLoop();
                Globals.runs--;
            }   
        }

        public static void Main(string[] args) {
            RunSimulation(args);            
        }
    }
}
