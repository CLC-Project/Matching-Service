using MatchingService.Entities;
using System.Collections.Generic;
using System.Linq;

namespace MatchingService.Dtos
{
    public class MatchingDto
    {
        public MatchingDto(Matching matching)
        {
            Id = matching?.Id.ToString();
            Destination = new DestinationDto(matching.Destination);
            MatchedDestinations = matching.MatchedDestinations.Select(x => new DestinationDto(x)).ToList();
        }

        public string Id { get; set; }

        public DestinationDto Destination { get; set; }

        public IList<DestinationDto> MatchedDestinations { get; set; }
    }
}
