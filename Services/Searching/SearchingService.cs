using RankingEntityes.Ranking_Entityes;
using System.Text.RegularExpressions;

namespace ModernSort.Services.Searching
{
    internal class SearchingService<T> where T : IHasSearchedKeyWords
    {
        private uint MinLenghtForSearchQueryWordsDecrission {  get; init; }
        public IEnumerable<T> SearchElements { get; set; }
        public SearchingService(uint minLenghtForSearchQueryWordsDecrission)
        {
            MinLenghtForSearchQueryWordsDecrission = minLenghtForSearchQueryWordsDecrission;
        }

        public SearchingService(uint minLenghtForSearchQueryWordsDecrission,IEnumerable<T> searchElements)
            : this(minLenghtForSearchQueryWordsDecrission)
        {
            SearchElements = searchElements;
        }

        public IEnumerable<T> SerchElements(string searchQuery)
        {
            if (searchQuery == String.Empty || searchQuery is null)
                return SearchElements.ToList();

            var result = SearchElements.ToList();
            result.Clear();

            foreach (var element in SearchElements)
            {
                if(IsElementSuit(element, searchQuery.Trim().ToLower()))
                    result.Add(element);

            }
            return result;
        }

        private bool IsSearchQueryMetchWithElementsKeyWords(List<string> elementsSearchWords, string searchQuery)
        {
            var keyWordsFullStringRow = string.Join(" ",elementsSearchWords);
            keyWordsFullStringRow = Regex.Replace(keyWordsFullStringRow, @"\p{P}", string.Empty);

            List<string> keyWordsSplited = new List<string>(keyWordsFullStringRow.ToLower().Split(" "));
            List<string> searchQueryWordsSplited = new List<string>(searchQuery.ToLower().Split(" "));

            foreach (var searchQueryWord in searchQueryWordsSplited)
            {
                if (IsSearchQueryWordMathcWithElementsKeyWords(searchQueryWord))
                    return true;
            }

            return false;

            bool IsSearchQueryWordMathcWithElementsKeyWords(string searchQueryWord)
            {
                do
                {
                    foreach (var elementKeyWord in keyWordsSplited)
                    {
                        if (elementKeyWord.Contains(searchQueryWord))
                            return true;
                    }
                    searchQueryWord = searchQueryWord.Remove(searchQueryWord.Length - 1);
                } 
                while (searchQueryWord.Length > MinLenghtForSearchQueryWordsDecrission);

                return false;
            }

        }

        private bool IsElementSuit(IHasSearchedKeyWords element, string searchQuery)
        {

            var elementsSearchWords = element.GetSearchDataWords();

            bool result = IsSearchQueryMetchWithElementsKeyWords(elementsSearchWords, searchQuery);

            return result;


        }
    }
}
