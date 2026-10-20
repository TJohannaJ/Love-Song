namespace Backend.Domain.DTOs;

public record NavigationGuideInfoDTO(
    string Route, 
    string PageName, 
    string Description
);