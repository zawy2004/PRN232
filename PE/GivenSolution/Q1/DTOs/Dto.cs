using Q1.Models;

namespace Q1.DTOs
{
    public class SecializationDto
    {
        public int SpecId { get; set; }

        public string? SpecName { get; set; }

        public string? Description { get; set; }

        public int? TrainerCount { get; set; }
    }
    public class CreateTrainerRequest
    {
        public string? TrainerName { get; set; }

        public string? Email { get; set; }

        public int? ExperienceYears { get; set; }

    }
    public class filterTrainerResponse
    {
        public int TrainerId { get; set; }

        public string? TrainerName { get; set; }

        public string? Email { get; set; }
        public int? ExperienceYears { get; set; }

        public int? TotalMinutesTrained { get; set; }
        public string? LastBookedMenber { get; set; }
        public List<string>? SpecializationList { get; set; }


    }
}
