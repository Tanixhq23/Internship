using AutoMapper;
using Entity; 

namespace DTO.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Employee, EmployeeDto>().ReverseMap();
            CreateMap<Department, DepartmentDto>().ReverseMap();
            CreateMap<LeaveRequest, LeaveRequestDto>().ReverseMap();
            CreateMap<LeaveType, LeaveTypeDto>().ReverseMap();
            CreateMap<LeaveBalance, LeaveBalanceDto>().ReverseMap();
            CreateMap<JobOpenings, JobOpeningsDto>().ReverseMap();
            CreateMap<Candidates, CandidatesDto>().ReverseMap();
            CreateMap<AttendanceRecord, AttendanceRecordDto>().ReverseMap();
            CreateMap<LifeCycleTask, LifeCycleTaskDto>().ReverseMap();
            CreateMap<User, RegisterDto>().ReverseMap();
            CreateMap<User, LoginDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}