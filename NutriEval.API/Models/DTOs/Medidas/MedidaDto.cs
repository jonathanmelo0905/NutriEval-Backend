namespace NutriEval.API.Models.DTOs.Medidas;

public class MedidaDto
{
    public Guid       Id               { get; init; }
    public Guid       TenantId         { get; init; }
    public Guid       ClienteId        { get; init; }
    public decimal?   Peso             { get; init; }
    public decimal?   PorcentajeGrasa  { get; init; }
    public decimal?   MasaMuscular     { get; init; }
    public decimal?   Imc              { get; init; }
    public decimal?   Cintura          { get; init; }
    public decimal?   Cadera           { get; init; }
    public decimal?   Pecho            { get; init; }
    public decimal?   BrazoDerecho     { get; init; }
    public decimal?   BrazoIzquierdo   { get; init; }
    public decimal?   PiernaDerecha    { get; init; }
    public decimal?   PiernaIzquierda  { get; init; }
    public DateOnly   Fecha            { get; init; }
    public string?    Notas            { get; init; }
    public DateTimeOffset CreatedAt    { get; init; }
}
