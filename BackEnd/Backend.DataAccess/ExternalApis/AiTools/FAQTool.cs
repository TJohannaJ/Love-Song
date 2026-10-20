using System.ComponentModel;

namespace Backend.Infrastructure.ExternalApis.Tools;

public class FAQTool
{
    [Description("Returns answers to Frequently Asked Questions (FAQ) about the LoveSong dating app, including theme settings, Spotify integration, AI bio generator, matching, and privacy.")]
    public string GetAnswer(
        [Description("The question, topic, or keyword the user is asking about (e.g. 'light mode', 'dark mode', 'theme', 'spotify premium', 'bio generator', 'match', 'undo dislike', 'search preferences').")]
        string topic)
    {
        var key = topic?.ToLower() ?? "";

        // 1. Theme & Appearance Settings
        if (key.Contains("theme") || key.Contains("light") || key.Contains("dark") || key.Contains("mode"))
        {
            return "You can easily toggle between Light and Dark mode by clicking the sun/moon icon at the bottom of the left sidebar navigation.";
        }

        // 2. Spotify & Music Integration
        if (key.Contains("match") && (key.Contains("music") || key.Contains("spotify") || key.Contains("taste")))
        {
            return "LoveSong builds a unique musical profile based on your top Spotify artists, genres, and tracks, then compares it with other users to find your best musical matches.";
        }

        if (key.Contains("premium") || key.Contains("free spotify"))
        {
            return "Spotify Premium requirement: No, a free Spotify account is completely sufficient to sync your music preferences with LoveSong.";
        }

        if (key.Contains("sync") || key.Contains("update music") || key.Contains("refresh song"))
        {
            return "Your music data syncs automatically, but you can manually re-sync it anytime from your Profile page.";
        }

        if (key.Contains("playlist") || key.Contains("private") || key.Contains("visible"))
        {
            return "Only your top played artists and songs are displayed on your profile. Your private playlists remain completely confidential.";
        }

        // 3. AI Features
        if (key.Contains("bio") || key.Contains("generator") || key.Contains("ai bio"))
        {
            return "On your Profile page, enter a few keywords and pick a tone (e.g., casual, romantic, witty). The AI Assistant will instantly draft a customized profile bio for you.";
        }

        if (key.Contains("assistant") || key.Contains("ai help") || key.Contains("sparkle") || key.Contains("widget"))
        {
            return "Clicking the floating ✨ button allows you to ask anything about navigating the app, finding features, or updating settings.";
        }

        // 4. Matching & Messaging
        if (key.Contains("chat") || key.Contains("message") || key.Contains("start conversation"))
        {
            return "Once you mutually like each other (a Match), the profile will appear on your 'Chat' page (/chat), where you can start a private chat.";
        }

        if (key.Contains("undo") || key.Contains("dislike") || key.Contains("pass") || key.Contains("accidental"))
        {
            return "You can view and undo your previous likes or dislikes on the Likes page (/likes) under the 'Your likes' or 'Your dislikes' buttons.";
        }

        // 5. Account & Settings
        if (key.Contains("preference") || key.Contains("filter") || key.Contains("age") || key.Contains("city") || key.Contains("distance"))
        {
            return "On the main Recommendations page (/), expand the 'Preference Settings' panel at the top to customize your age, city, and music filters.";
        }

        // Default General FAQ Answer
        return "LoveSong is a music-based dating app that connects people through shared Spotify tastes, AI-assisted navigation, and smart matching features.";
    }
}