using System.ComponentModel;
using Backend.Domain.DTOs;

namespace Backend.Infrastructure.ExternalApis.Tools;

public class AppNavigationTool 
{
    [Description("Returns navigation guide and page routes for the LoveSong dating app.")]
    public NavigationGuideInfoDTO GetPageNavigationInfo(
        [Description("The topic, page, or feature the user is looking for, e.g., 'profile', 'likes', 'chat', 'recommendations', 'spotify'")] string topic)
    {
        var key = topic?.ToLower() ?? "";

        if (key.Contains("profile") || key.Contains("bio") || key.Contains("edit") || key.Contains("introduction"))
        {
            return new NavigationGuideInfoDTO(
                Route: "/profile",
                PageName: "Profile Page",
                Description: "Go to the Profile tab on the left sidebar to edit your info, generate an AI bio, or link Spotify."
            );
        }

        if (key.Contains("undo") && (key.Contains("like") || key.Contains("dislike")))
        {
            return new NavigationGuideInfoDTO(
                Route: "/likes",
                PageName: "Likes Page",
                Description: "Go to the Likes tab on the left sidebar, then click 'Your likes' or 'Your dislikes' at the top left to view and undo them."
            );
        }

        if (key.Contains("like") || key.Contains("who"))
        {
            return new NavigationGuideInfoDTO(
                Route: "/likes",
                PageName: "Likes Page",
                Description: "Go to the Likes tab on the left sidebar. Under 'They liked you', you can view people who liked you and match with them."
            );
        }

        if (key.Contains("chat") || key.Contains("message") || key.Contains("text") || key.Contains(("match")))
        {
            return new NavigationGuideInfoDTO(
                Route: "/chat",
                PageName: "Chat Page",
                Description: "Go to the Chat tab on the left sidebar to view your matches and send messages."
            );
        }

        if (key.Contains("spotify") || key.Contains("music") || key.Contains("song"))
        {
            return new NavigationGuideInfoDTO(
                Route: "/profile",
                PageName: "Profile Page",
                Description: "Go to the Profile tab and click 'Connect Spotify Account' to sync your music preferences."
            );
        }

        return new NavigationGuideInfoDTO(
            Route: "/",
            PageName: "Home / Recommendations",
            Description: "Go to the Home tab on the left sidebar to view recommendations and like or dislike profiles."
        );
    }
}