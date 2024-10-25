public class CellNumber
{
    public int x { get;}
    public int y { get;}

    public CellNumber(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override bool Equals(object obj)
    {
        if (obj is CellNumber other)
        {
            return this.x == other.x && this.y == other.y;
        }

        return false;
    }

    // ハッシュコードをオーバーライド
    public override int GetHashCode()
    {
        return this.x.GetHashCode() ^ this.y.GetHashCode();
    }

    // デバッグのためにToStringをオーバーライド
    public override string ToString()
    {
        return $"({this.x}, {this.y})";
    }

    public static CellNumber operator +(CellNumber a, CellNumber b)
    {
        return new CellNumber(a.x + b.x, a.y + b.y);
    }

    public static CellNumber operator -(CellNumber a, CellNumber b)
    {
        return new CellNumber(a.x - b.x, a.y - b.y);
    }

    // ==演算子のオーバーロード（等価比較）
    public static bool operator ==(CellNumber a, CellNumber b)
    {
        // 両方がnullの場合は等しいと見なす
        if (ReferenceEquals(a, b))
        {
            return true;
        }

        // 一方がnullの場合は等しくない
        if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
        {
            return false;
        }

        return a.Equals(b);
    }

    // !=演算子のオーバーロード（不等比較）
    public static bool operator !=(CellNumber a, CellNumber b)
    {
        if (ReferenceEquals(a, b))
        {
            return false;
        }

        if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
        {
            return true;
        }

        return !a.Equals(b);
    }

    // これはnullを表す特別なstatic readonly変数
    public static readonly CellNumber Null = null;
}

