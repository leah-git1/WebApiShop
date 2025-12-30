namespace DTOs
{
    public record OrderDTO
    (
        int OrderId,
        DateOnly OrderDate,
        double OrderSum
    );
}
