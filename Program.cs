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
        public const int NEXT_ROUND = 3;
        public const int BREAK = 4;
        public static bool doInsurance = false;
        public static bool doubleAllowedAfterSplit = true;
        public static bool surrendersAllowed = true;
        public static bool houseStaysOnSoft17 = true;
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

        public Hand() : this(0.00m) {}
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
        public void Clear() {
            this.cards = new List<int>();
            this.bet = 0.00m;
            this.insuranceBet = 0.00m;
            this.hardVal = 0;
            this.realVal = 0;
            this.hasAce = false;
            this.isSplit = false;
            this.isDoubled = false;
            this.isDealer = false;
        }
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
        public const int H =  1; // HIT
        public const int S =  2; // STAY        
        public const int DH = 3; // DOUBLE, else HIT
        public const int DS = 4; // DOUBLE, else STAY
        public const int P  = 5; // SPLIT
        public const int PH = 6; // SPLIT if double after split allowed, else HIT
        public const int RH = 7; // SURRENDER if allowed, else HIT
        public const int K  = 0;
        private static int[,] hardStrategy = {
        //   0  1  2  3  4  5  6  7  8  9 10
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, //0
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, //1
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, //2
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, //3
            {0, H, H, H, H, H, H, H, H, H, H}, //4
            {0, H, H, H, H, H, H, H, H, H, H}, //5
            {0, H, H, H, H, H, H, H, H, H, H}, //6
            {0, H, H, H, H, H, H, H, H, H, H}, //7
            {0, H, H, H, H, H, H, H, H, H, H}, //8
            {0, H, H,DH,DH,DH,DH, H, H, H, H}, //9
            {0, H,DH,DH,DH,DH,DH,DH,DH,DH, H}, //10
            {0, H,DH,DH,DH,DH,DH,DH,DH,DH,DH}, //11
            {0, H, H, H, S, S, S, H, H, H, H}, //12
            {0, H, S, S, S, S, S, H, H, H, H}, //13            
            {0, H, S, S, S, S, S, H, H, H, H}, //14
            {0, H, S, S, S, S, S, H, H, H,RH}, //15
            {0,RH, S, S, S, S, S, H, H,RH,RH}, //16
            {0, S, S, S, S, S, S, S, S, S, S}, //17
            {0, S, S, S, S, S, S, S, S, S, S}, //18
            {0, S, S, S, S, S, S, S, S, S, S}, //19
            {0, S, S, S, S, S, S, S, S, S, S}, //20
            {0, S, S, S, S, S, S, S, S, S, S}  //21  
        };
        private static int[,] softStrategy = {
        //   0  1  2  3  4  5  6  7  8  9 10
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, //0
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, //1
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, //2 
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, //3           
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, //4
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, //5
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, //6
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, //7
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, //8
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, //9
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, //10
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, //11
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, //12
            {0, H, H, H, H,DH,DH, H, H, H, H}, //13
            {0, H, H, H, H,DH,DH, H, H, H, H}, //14
            {0, H, H, H,DH,DH,DH, H, H, H, H}, //15
            {0, H, H, H,DH,DH,DH, H, H, H, H}, //16
            {0, H, H,DH,DH,DH,DH, H, H, H, H}, //17
            {0, H, S,DS,DS,DS,DS, S, S, H, H}, //18
            {0, S, S, S, S, S, S, S, S, S, S}, //19
            {0, S, S, S, S, S, S, S, S, S, S}, //20
            {0, S, S, S, S, S, S, S, S, S, S}, //21
        };
        private static int[,] splitStrategy = {
        //    0  1  2  3  4  5  6  7  8  9 10
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, //0
            {0, P, P, P, P, P, P, P, P, P, P}, //1
            {0, H,PH,PH, P, P, P, P, H, H, H}, //2
            {0, H,PH,PH, P, P, P, P, H, H, H}, //3
            {0, H, H, H, H,PH,PH, H, H, H, H}, //4
            {0, K, K, K, K, K, K, K, K, K, K}, //5
            {0, H,PH, P, P, P, P, H, H, H, H}, //6
            {0, H, P, P, P, P, P, P, H, H, H}, //7
            {0, P, P, P, P, P, P, P, P, P, P}, //8
            {0, S, P, P, P, P, P, S, P, P, S}, //9
            {0, K, K, K, K, K, K, K, K, K, K}, //10
        };

        public static int Hard(int r, int c) {
            return hardStrategy[r,c];
        }

        public static int Soft(int r, int c) {
            return softStrategy[r,c];
        }

        public static int Split(int r, int c) {
            return splitStrategy[r,c];
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

        public static String CardsToString<T>(IEnumerable<T> cards) {
            String str = "[";
            foreach (T item in cards) {
                str += item.ToString() + ", ";
            }
            return str + "]";
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

        private static void SplitHand(List<Hand> hands, int index) {
            Hand oldHand = hands[index];
            decimal bet = oldHand.bet;
            Hand splitHand = new Hand(bet);
            splitHand.isSplit = true;
            splitHand.DealCard(oldHand.cards[1]);
            hands.Insert(index+1, splitHand);

            int oldCard = oldHand.cards[0];
            oldHand.cards.Clear();
            oldHand.realVal = 0;
            oldHand.hardVal = 0;
            oldHand.isSplit = true;
            oldHand.DealCard(oldCard);
        }

        private static int HandLoop(List<Hand> hands, Hand hand, int decision) {
            Hand dealer = hands[hands.Count()-1];
            int upCard = dealer.cards[0];
            HashSet<int> endingDecisions = new HashSet<int>()
            {Strategy.S, Strategy.DS};
            HashSet<int> doubleDecisions = new HashSet<int>()
            {Strategy.DH, Strategy.DS};
            while (hand.realVal<21 && !endingDecisions.Contains(decision)) {
                if (!hand.IsSoft() || hand.realVal<13) 
                    decision = Strategy.Hard(hand.hardVal, upCard);
                else
                    decision = Strategy.Soft(hand.realVal, upCard);
                if (decision == Strategy.H) {
                    hand.DealCard(Globals.shoe.Pop());
                } else if (decision == Strategy.S) {
                    //pass
                } else if (doubleDecisions.Contains(decision)) {
                    if (hand.cards.Count() == 2) {
                        // Double
                        hand.bet *= 2;
                        hand.DealCard(Globals.shoe.Pop());
                        hand.isDoubled = true;
                        break;
                    } else if (decision == Strategy.DH){
                        hand.DealCard(Globals.shoe.Pop());
                    }
                } else if (decision == Strategy.RH) {
                    // Surrender or hit
                    if (hand.cards.Count() == 2) {
                        break;
                    } else {                        
                        hand.DealCard(Globals.shoe.Pop());
                    }
                }
            }
            return decision;
        }

        private static void RoundLoop(List<Hand> hands) {
            Hand dealer = hands[hands.Count()-1];
            int upCard = dealer.cards[0];
            int numPlayerHands = hands.Count() - 1;
            int i = 0; //i is index of the current hand
            while (i < numPlayerHands) {
                Hand hand = hands[i];

                // Deal card if hand is result of a split
                if (hand.cards.Count() == 1) {
                    hand.DealCard(Globals.shoe.Pop());
                    if (hand.cards[0] == 1) {
                        i++;
                        continue;
                    }
                }

                //Check for split
                int decision = 99; //nothing
                if (hand.cards[0] == hand.cards[1]) {
                    decision = Strategy.Split(hand.cards[0], upCard);
                    switch (decision) {
                        case Strategy.P:
                            SplitHand(hands, i);
                            continue;
                        case Strategy.H:
                            hand.DealCard(Globals.shoe.Pop());
                            break;
                        case Strategy.K:
                            break;
                        case Strategy.PH:
                            if (Globals.doubleAllowedAfterSplit) {
                                SplitHand(hands, i);
                                continue;
                            } else {
                                hand.DealCard(Globals.shoe.Pop());
                            }
                            break;
                        case Strategy.S:
                            break;
                    }
                }

                //Hand loop
                decision = HandLoop(hands, hand, decision);

                // if busted or surrender
                if (hand.realVal>21) {
                    decimal surrenderedAmt = Math.Ceiling(hand.bet) / 2;
                    Stats.playerBank -= surrenderedAmt;
                    Stats.houseBank += surrenderedAmt;
                    Stats.surrenders++;
                    Console.WriteLine("[Hand{0}: {1}={2}] {}", i, hand.realVal, CardsToString(hand.cards), "surrendered");
                    hands.Remove(hand);
                } else if (decision==Strategy.RH && hand.cards.Count()==2) {
                    Stats.playerBank -= hand.bet;
                    Stats.houseBank += hand.bet;
                    Stats.losses++;
                    if (hand.isDoubled)
                        Stats.losses++;  //Doubles count as two losses
                    Console.WriteLine("[Hand{0}: {1}={2}] {}", i, hand.realVal, CardsToString(hand.cards), "busted");
                    hands.Remove(hand);
                } else {
                    // Stay
                    i++;
                }
            }
        }

        public static void ShoeLoop() {
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

                // Round loop
                if (hands.Count() > 1) {
                    RoundLoop(hands);
                }

                if (hands.Count() > 1) {
                    // Deal to house and evaluate
                    // pay and take
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
                ShoeLoop();
                Globals.runs--;
            }   
        }

        public static void TestArea() {

        }
        public static void Main(string[] args) {
            // TestArea();
            RunSimulation(args); 
        }        
    }
}
