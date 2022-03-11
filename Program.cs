using System.Collections.Generic;

namespace bch_bingo
{
    class Program
    {

        class Vocabulary {
            public Vocabulary(string folder = "vocabulary") {
                var directoryInfo = new System.IO.DirectoryInfo(folder);
                var fileInfoArray = directoryInfo.GetFiles("*.png");
                foreach(var fileInfo in fileInfoArray ) {
                    var name = System.IO.Path.GetFileNameWithoutExtension(fileInfo.Name);
                    var fileStream = new System.IO.FileStream(fileInfo.FullName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                    var bitmap = new System.Drawing.Bitmap(fileStream);
                    _bitmaps.Add(bitmap);
                    _names.Add(name);
                    ++_count;
                }            
            }

            int _count;
            public int count {get => _count;}

            List<System.Drawing.Bitmap> _bitmaps = new List<System.Drawing.Bitmap>();
            public System.Drawing.Bitmap GetBitmapByIndex(int index) {return _bitmaps[index];}

            List<string> _names = new List<string>();
            public string GetNameByIndex(int index) {return _names[index];}
        }

        class Card {
            int _dimInBoxes;
            public int dimInBoxes {get => _dimInBoxes;}

            List<int> _vocabIndices = new List<int>();
            System.Drawing.Bitmap _bitmap;
            public System.Drawing.Bitmap bitmap {get => _bitmap;}

            public Card(int dimInBoxes, int width, int height, Vocabulary vocab, int padding = 10) {
                _dimInBoxes = dimInBoxes;
                var numBoxes = _dimInBoxes * _dimInBoxes;
                var random = new System.Random();

                for (var i = 0; i < numBoxes; ++i) {
                    var number = random.Next(1, vocab.count);
                    while (_vocabIndices.Contains(number)) {
                        number = random.Next(1, vocab.count);
                    }
                    _vocabIndices.Add(number);
                }

                _bitmap = new System.Drawing.Bitmap(width, height);
                using (var graphics = System.Drawing.Graphics.FromImage(_bitmap))
                {
                    using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.White)) {
                        var rect = new System.Drawing.Rectangle(0, 0, width, height);
                        graphics.FillRectangle(brush, rect);
                    }

                    var dw = width / (float)_dimInBoxes;
                    var dh = height / (float)_dimInBoxes;

                    for (var i = 0; i < _dimInBoxes + 1; ++i) {
                        var pen = new System.Drawing.Pen(System.Drawing.Color.Black, 5);
                        var x = i * dw;
                        var y = i * dh;
                        graphics.DrawLine(pen, 0, y, width, y);
                        graphics.DrawLine(pen, x, 0, x, height);
                    }

                    for (var row = 0; row < _dimInBoxes; ++row) {
                        for (var col = 0; col < _dimInBoxes; ++col) {
                            var i = row * _dimInBoxes + col;
                            var vocabIndex = _vocabIndices[i];
                            var vocabBitmap = vocab.GetBitmapByIndex(vocabIndex);
                            var boxWidth = dw - (2 * padding);
                            var boxHeight = dh - (2 * padding);
                            var widthRatio = boxWidth / vocabBitmap.Width;
                            var heightRatio = boxHeight / vocabBitmap.Height;
                            var ratio = Math.Min(widthRatio, heightRatio);
                            var resizedWidth = (int)(ratio * vocabBitmap.Width);
                            var resizedHeight = (int)(ratio * vocabBitmap.Height);
                            var x = (int)((col + 0.5f) * dw) - resizedWidth / 2;
                            var y = (int)((row + 0.5f) * dh) - resizedHeight / 2;
                            graphics.DrawImage(vocabBitmap, x, y, resizedWidth, resizedHeight);
                        }
                    }    
                }
            }
#if false
            private System.Drawing.Font GetAdjustedFont(System.Drawing.Graphics g, string graphicString, System.Drawing.Font originalFont, int containerWidth, int maxFontSize, int minFontSize, bool smallestOnFail) {
                System.Drawing.Font testFont = originalFont;
                for (int adjustedSize = maxFontSize; adjustedSize >= minFontSize; adjustedSize--) {
                    testFont = new System.Drawing.Font(originalFont.Name, adjustedSize, originalFont.Style);
                    System.Drawing.SizeF adjustedSizeNew = g.MeasureString(graphicString, testFont);
                    if (containerWidth > Convert.ToInt32(adjustedSizeNew.Width)) {
                        // Good font, return it
                        return testFont;
                    }
                }

                // If you get here there was no fontsize that worked
                // return minimumSize or original?
                if (smallestOnFail) {
                    return testFont;
                } else {
                    return originalFont;
                }
            }            

            private void _DrawString(System.Drawing.Graphics graphics, String str, System.Drawing.SizeF bounds, String fontFamilyName) {
                int bestSize = 1;
                for (int thisSize = bestSize; ; ++thisSize) {
                    var font = new System.Drawing.Font(fontFamilyName, thisSize);
                    System.Drawing.SizeF thisBounds = graphics.MeasureString(str, font);
                    if ((thisBounds.Width > bounds.Width) || (thisBounds.Height > bounds.Height)) {
                        break;
                    }
                }

            }
#endif            
        }

        static void CreateCallList(Vocabulary vocab) {
            var rnd = new System.Random();
            var c = vocab.count;
            var list = new List<int>();
            for (var i = 0; i < c; ++i) list.Add(i);
            for (var i = c; i > 0; i--) {
                var j = rnd.Next(0, i);
                var temp = list[0];
                list[0] = list[j];
                list[j] = temp;
            }
            
            using System.IO.StreamWriter sw = new("calllist.txt");
            var n = 0;
            foreach (var index in list) {
                var name = vocab.GetNameByIndex(index);
                var str = String.Format("{0}:{1}", n,name);
                sw.WriteLine(str);
                ++n;
            }

        }

        static void CreateCards(Vocabulary vocab)
        {
            var count = 22;
            for (var i = 1; i <= count; ++i) {
                var card = new Card(5, 2480, 3508, vocab);
                card.bitmap.Save($"out{i,0:00}.png", System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        static void Main(string[] args)
        {
            var vocab = new Vocabulary();
            
            CreateCards(vocab);
            CreateCallList(vocab);
        }


    }
}