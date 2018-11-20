using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ParkShark.Data.Model;
using ParkShark.Domain;
using ParkShark.Infrastructure;
using ParkShark.Infrastructure.Exceptions;

namespace ParkShark.Services
{
    public interface IMemberService : IParkSharkService
    {
        Task<Member> CreateNewMember(Member member);
        Task<IEnumerable<Member>> GetAllMembers();
        Task<Member> GetMember(int id);
    }

    public class MemberService : IMemberService
    {
        private readonly ParkSharkDbContext context;

        public MemberService(ParkSharkDbContext context)
        {
            this.context = context;
        }

        public async Task<Member> CreateNewMember(Member member)
        {
            await context.Members.AddAsync(member);

            if (await context.SaveChangesAsync() == 0)
                throw new PersistenceException("Member was not created");

            await context.Entry(member).Reference(p => p.Contact).LoadAsync();

            return member;
        }
        public async Task<IEnumerable<Member>> GetAllMembers()
        {
            return await context.Set<Member>()
                .Include(m => m.Contact)
                .Include(m => m.RelatedMemberShipLevel)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Member> GetMember(int id)
        {
            return await context.Set<Member>()
                .Include(m => m.Contact)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);
        }
    }
}
