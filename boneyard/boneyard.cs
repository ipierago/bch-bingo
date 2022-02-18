#if false
        class Card {
            public Card(int dim, List<int> numbers) {
                _dim = dim;
                _numbers = numbers;
            }

            int _dim;
            public int dim {get {return _dim;}}

            List<int> _numbers;
            public int GetNumberByIndex(int index) {
                return _numbers[index];
            }
        }

        class CardSet {
            int _dim;
            public int dim {get {return _dim;}}

            int _maxNumber;
            public int maxNumber {get {return _maxNumber;}}

            public CardSet(int dim = 5, int maxNumber = 30, int numCards = 1) {
                _dim = dim;
                _maxNumber = maxNumber;
                var random = new Random();
                for (var i = 0; i < numCards; ++i) {
                    var listOfNumbers = new List<int>();
                    var numBoxes = _dim * _dim;
                    for (var j = 0; j < numBoxes; ++j) {
                        var number = random.Next(1, _maxNumber);
                        while (listOfNumbers.Contains(number)) {
                            number = random.Next(1, _maxNumber);
                        }
                        listOfNumbers.Add(number);
                    }
                    var card = new Card(_dim, listOfNumbers);
                    _cards.Add(card);
                }
            }

            List<Card> _cards = new List<Card>();
            public int GetCardCount() {return _cards.Count;}
            public Card GetCardByIndex(int index) {return _cards[index];}
        }

        static void DumpCardSet(CardSet cardSet) {
            var cardCount = cardSet.GetCardCount();
            for (var i = 0; i < cardCount; ++i) {
                var card = cardSet.GetCardByIndex(i);
                var numBoxes = card.dim * card.dim;
                for (var j = 0; j < numBoxes; ++j) {
                    var number = card.GetNumberByIndex(j);
                    if (j > 0) Console.Write(", ");
                    Console.Write($"{number}");
                }
                Console.WriteLine();                
            }
        }
#endif
