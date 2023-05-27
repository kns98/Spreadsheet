public class CellCoordinates
{
    public int RowNumber { get; }
    public int ColNumber { get; }

    public CellCoordinates(int rowNumber, int colNumber)
    {
        RowNumber = rowNumber;
        ColNumber = colNumber;
    }

    public CellCoordinates(string cellCoordinate)
    {
        if (TryParseCellCoordinate(cellCoordinate, out int row, out int col))
        {
            RowNumber = row;
            ColNumber = col;
        }
        else
        {
            throw new ArgumentException("Invalid cell coordinate format.");
        }
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

    private bool TryParseCellCoordinate(string cellCoordinate, out int row, out int col)
    {
        row = -1;
        col = -1;

        if (cellCoordinate.Length < 2)
        {
            return false;
        }

        if (cellCoordinate[0] != 'R' || cellCoordinate[1] != 'C')
        {
            return false;
        }

        string rowStr = string.Empty;
        string colStr = string.Empty;

        int i = 2;
        while (i < cellCoordinate.Length && Char.IsDigit(cellCoordinate[i]))
        {
            rowStr += cellCoordinate[i];
            i++;
        }

        if (i >= cellCoordinate.Length || cellCoordinate[i] != 'C')
        {
            return false;
        }

        i++;
        while (i < cellCoordinate.Length && Char.IsDigit(cellCoordinate[i]))
        {
            colStr += cellCoordinate[i];
            i++;
        }

        if (!int.TryParse(rowStr, out row) || !int.TryParse(colStr, out col))
        {
            return false;
        }

        return true;
    }
}
