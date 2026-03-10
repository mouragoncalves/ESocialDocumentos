using ESocial.Application.DTOs;
using MediatR;

namespace ESocial.Application.UseCases.ConsultarIdentificadores;

public record ConsultarIdentificadoresQuery(ConsultaIdentificadoresDto Consulta) : IRequest<ConsultarIdentificadoresResult>;
