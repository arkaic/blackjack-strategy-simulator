using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// using NUnit.Framework;

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
        public static Random rng = null;
        public static decimal betAmt = 5.00m;
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

        // Range of values for card = [1,10]
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
        public static decimal netGains = 0.00m;
        public static int wins = 0;
        public static int losses = 0;
        public static int pushes = 0;
        public static int surrenders = 0;
        private static decimal minBank = 0.00m;
        private static decimal minRoundBank = 0.00m;

        //actual value = current (+/-)bankroll divided by total wagers made?
        public static decimal ActualEdge() {            
            // implement
            return 0.00m;
        }

        public static void Pay(decimal amount) {
            netGains += amount;
        }

        public static void Take(decimal amount) {
            netGains -= amount;
        }

        public static void Push(decimal bet) {

        }

        // Updates total bets made by comparing to bankroll of round.
        public static void MakeBet(decimal bet) {
            minRoundBank += bet; //minRoundBank is positive
            if (minBank < minRoundBank)
                minBank = minRoundBank;
        }

        // Called at the beginning of every round loop to keep track of player's
        // current bankroll. Then used to also track total bets that are made
        public static void SetMinimumRoundBankroll() {
            if (netGains >= 0) 
                minRoundBank = 0;
            else
                minRoundBank = Math.Abs(netGains); 
        }

        public static decimal GetTotalBetsMade() {
            return minBank;
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
        //   0  1  2  3  4  5  6  7  8  9  10
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
        private static int[] insuranceStrategy = {
        //  0  1  2  3  4  5  6  7  8  9
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1            
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

        public static int Insure(int r) {
            return insuranceStrategy[r];
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

        // Decrementing downwards, it chooses value at top index and 
        // puts it in a random lower index.
        public void Shuffle() {
            if (Globals.rng == null) Globals.rng = new Random((int)DateTime.Now.Ticks);
            // Random rng = new Random(Guid.NewGuid().GetHashCode());
            Random rng = new Random((int)DateTime.Now.Ticks);
            // Random rng = Globals.rng;            
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
            return this.cards.Count();
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

        private static void Exit(String msg) {
            System.Console.WriteLine(msg);
            System.Environment.Exit(-1);
        }

        private static String CardsToString<T>(IEnumerable<T> cards) {
            String s = "[";
            foreach (T item in cards)
                s += item.ToString() + ", ";            
            return s + "]";
        }

        private static void PrintHands(List<Hand> hands) {
            if (hands.Count() < 2) {
                Console.WriteLine("No more player hands");
                return;
            } else {
                for (int i = 0; i < hands.Count()-1; i++) {
                    String splitStr = (hands[i].isSplit) ? "(split)" : "";
                    Console.WriteLine("Hand{0} = {1}: {2} {3}", i, hands[i].realVal, CardsToString(hands[i].cards), splitStr);
                }

                Console.WriteLine("DEALER: {0}", CardsToString(hands[hands.Count()-1].cards));
            }
        }

        private static void SetupHands(List<Hand> hands, int numPlayerHands, 
                decimal startBet) {
            for (int i = 0; i < numPlayerHands; i++) {
                hands.Add(new Hand(startBet));
                Stats.MakeBet(startBet);    
            }
            Hand dealer = new Hand();
            dealer.isDealer = true;
            hands.Add(dealer);
        }

        private static void DealStartingCards(List<Hand> hands) {
            Globals.handsWithBJ = new List<Hand>();
            foreach (Hand hand in hands) {
                hand.DealCard(Globals.shoe.Pop());
            }
            foreach (Hand hand in hands) {
                hand.DealCard(Globals.shoe.Pop());
                if (hand.realVal==21 && !hand.isDealer) {
                    // If hand is blackjack and is not dealer
                    Globals.handsWithBJ.Add(hand);
                }
            }
        }

        // Take all bets except for ties
        private static void TakeAllBets(List<Hand> hands) { 
            int numPlayerHands = hands.Count() - 1;
            for (int i = 0; i < numPlayerHands; i++) {
                if (hands[i].realVal < 21) {
                    Stats.Take(hands[i].bet);
                    Stats.losses++;
                } else if (hands[i].realVal == 21) {
                    Stats.Push(hands[i].bet);
                    Stats.pushes++;
                }
            }
        }

        private static void PayAllInsurances(List<Hand> hands) {
            int numPlayerHands = hands.Count() - 1;
            for (int i = 0; i < numPlayerHands; i++) {
                if (hands[i].insuranceBet > 0) {
                    decimal payout = 2 * hands[i].insuranceBet;
                    Stats.Pay(payout);
                }
            }
        }

        private static void TakeAllInsurances(List<Hand> hands) {
            int numPlayerHands = hands.Count() - 1;
            for (int i = 0; i < numPlayerHands; i++) {
                Stats.Take(hands[i].insuranceBet);
                hands[i].insuranceBet = 0.00m;
            }
        }

        private static void PayAllBlackjacks(List<Hand> hands) {
            for (int i = 0; i < Globals.handsWithBJ.Count(); i++) {
                // Round every blackjack payout up to the nearest half.
                Hand bjHand = Globals.handsWithBJ[i];
                decimal payout = Math.Ceiling((decimal)1.5 * bjHand.bet * 2) / 2;
                Stats.Pay(payout);
                Stats.wins++;
                hands.Remove(bjHand);
            }
        }

        private static void SetupInsurances(List<Hand> hands) {
            for (int i = 0; i < hands.Count()-1; i++) {
                if (Strategy.Insure(hands[i].realVal) == 1) { 
                    hands[i].insuranceBet = (decimal) Math.Ceiling(hands[i].bet) / 2;
                    Stats.MakeBet(hands[i].insuranceBet);
                }
            }
        }

        private static bool CheckDealerForBlackjack(List<Hand> hands) {
            Hand dealer = hands[hands.Count()-1];
            if (dealer.cards[0]==10 && dealer.cards[1]==1) {
                TakeAllBets(hands);                
                return true;
            } else if (dealer.cards[0] == 1) {
                if (Globals.doInsurance) {
                    Console.WriteLine("Doing insurance");
                    SetupInsurances(hands);
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
            Stats.MakeBet(bet);
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

        /*======================================================================
                                Hand Loop
         =====================================================================*/
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
                    //Stay
                } else if (doubleDecisions.Contains(decision)) {
                    if (hand.cards.Count() == 2) {
                        // Double
                        decimal doubleBet = hand.bet * 2;
                        hand.bet += doubleBet;
                        Stats.MakeBet(doubleBet);
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

        /*======================================================================
                                Round Loop
         =====================================================================*/
        private static void RoundLoop(List<Hand> hands) {
            Hand dealer = hands[hands.Count()-1];
            int upCard = dealer.cards[0];
            int i = 0; //i is index of the current hand
            while (i < hands.Count()-1) {
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

                /*--------------------------------------------------------------
                                   Hand loop call
                --------------------------------------------------------------*/
                decision = HandLoop(hands, hand, decision);

                // if busted or surrender
                if (hand.realVal>21) {
                    Stats.Take(hand.bet);
                    Stats.losses++;
                    // if (hand.isDoubled) Stats.losses++;  //Doubles count as two losses
                    Console.WriteLine("[Hand{0}: {1}={2}] {3}", i, hand.realVal, CardsToString(hand.cards), "busted");
                    hands.Remove(hand);
                } else if (decision==Strategy.RH && hand.cards.Count()==2) {
                    decimal surrenderedAmt = Math.Ceiling(hand.bet) / 2;
                    Stats.Take(surrenderedAmt);
                    Stats.surrenders++;
                    Console.WriteLine("[Hand{0}: {1}={2}] {3}", i, hand.realVal, CardsToString(hand.cards), "surrendered");
                    hands.Remove(hand);
                } else {
                    // Stay
                    i++;
                }
            }
        }

        /*======================================================================
                                Dealer Loop
         =====================================================================*/
        private static void DealerLoop(List<Hand> hands) {
            Hand dealer = hands[hands.Count()-1];
            // Deal to dealer and evaluate
            if (Globals.houseStaysOnSoft17) {
                while (dealer.realVal < 17)
                    dealer.DealCard(Globals.shoe.Pop());
            } else {
                while (dealer.hardVal < 17)
                    dealer.DealCard(Globals.shoe.Pop());
            }

            Console.WriteLine("ENDING HANDS");
            PrintHands(hands);

            // Evaluate, pay and take
            if (dealer.realVal > 21) {
                // pay all
                Console.WriteLine("Dealer busts, paying....");
                for (int i = 0; i < hands.Count()-1; i++) {
                    Stats.Pay(hands[i].bet);
                    Stats.wins++;
                    if (hands[i].isDoubled) Stats.wins++;
                }
            } else {
                for (int i = 0; i < hands.Count()-1; i++) {
                    if (hands[i].realVal > dealer.realVal) {
                        Stats.Pay(hands[i].bet);
                        Stats.wins++;
                        // if (hands[i].isDoubled) Stats.wins++;
                    } else if (hands[i].realVal < dealer.realVal) {
                        Stats.Take(hands[i].bet);
                        Stats.losses++;
                        // if (hands[i].isDoubled) Stats.losses++;
                    } else {
                        Stats.pushes++;
                        Stats.Push(hands[i].bet);
                    }
                }
            }    
            // Do some strategy stuff            
        }

        /*======================================================================
                                SHOE Loop
         =====================================================================*/
        private static void ShoeLoop() {
            int shoeSize = Globals.shoe.Count();
            Globals.shoe.Pop(); // Burn card

            Console.WriteLine("\n\n\n-----------------------------------------");
            Console.WriteLine("NEW SHOE: {0} cards", shoeSize);
            Console.WriteLine("-----------------------------------------");

            //Rounds Loop
            while (Globals.shoe.Count() >= (shoeSize/5)) {
                // Set up hands
                Stats.SetMinimumRoundBankroll();
                List<Hand> hands = new List<Hand>();                
                int numPlayerHands = 2; // Soon to be affected by strategy
                SetupHands(hands, numPlayerHands, Globals.betAmt);
                Hand dealer = hands[hands.Count()-1];

                // Deal cards to every hand
                DealStartingCards(hands);
                Console.WriteLine("\nSTARTING HANDS");
                Console.WriteLine("HOUSE: {0}", dealer.cards[0]);
                PrintHands(hands);
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

                // Dealer loop
                if (hands.Count() > 1) {
                    DealerLoop(hands);
                }
            }
        }

        private static void PrintStatistics() {
            Console.WriteLine("\n\n----------------------------------------");
            Console.WriteLine("Wins: {0}\nLosses: {1}\nPushes: {2}\nSurrenders: {3}", Stats.wins, Stats.losses, Stats.pushes, Stats.surrenders);
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("Bet per hand: ${0}", Globals.betAmt);
            if (Stats.netGains < 0)
                Console.WriteLine("Net loss of ${0}", Stats.netGains);
            else
                Console.WriteLine("Net gain of ${0}", Stats.netGains);
            Console.WriteLine("Total minimum bankroll: {0:C}", Stats.GetTotalBetsMade());
            Console.WriteLine("{0:N2} times the bankroll net gain/loss", Stats.netGains/Stats.GetTotalBetsMade());
        }

        /*======================================================================
                                    Main Function
         =====================================================================*/
        private static void RunSimulation(string[] args) {
            Stats.netGains = 0.00m;
            Globals.numDecks = 6;
            Globals.shoe = new ShoeList();
            if (args.Count() == 1)
                Globals.runs = Convert.ToInt32(args[0]);
            else 
                Globals.runs = 1;
            Console.WriteLine(Globals.runs + " runs");

            // Shoe runs loop
            while (Globals.runs > 0) {
                Globals.shoe.Generate(Globals.numDecks);
                Globals.shoe.Shuffle();          
                ShoeLoop();
                Globals.runs--;
            }   

            PrintStatistics();
        }

        public static void Main(string[] args) {
             RunSimulation(args);
        }        
    }
}