namespace DTOs
{
    public record LessInfoOrderDTO
    (
        int OrderId,
        DateOnly OrderDate,
        double OrderSum
    );
    public record MoreInfoOrderDTO(
        int OrderId,
        DateOnly OrderDate,
        double OrderSum
        
    );
}
