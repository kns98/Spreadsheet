
namespace MicroSpread
{
    public class CellCoordinates
    {
        public int RowNumber { get; }
        public int ColNumber { get; }

        public CellCoordinates(int rowNumber, int colNumber)
        {
            RowNumber = rowNumber;
            ColNumber = colNumber;
        }

        public override string ToString()
        {
            return $"Row {RowNumber}, Column {ColNumber}";
        }

        public string Id()
        {
            int row = RowNumber;
            int column = ColNumber;

            string columnLetters = string.Empty;

            // Convert column index to column letters
            while (column > 0)
            {
                int remainder = (column - 1) % 26;
                columnLetters = (char)('A' + remainder) + columnLetters;
                column = (column - 1) / 26;
            }

            return $"{columnLetters}{row}";
        }
    }

}