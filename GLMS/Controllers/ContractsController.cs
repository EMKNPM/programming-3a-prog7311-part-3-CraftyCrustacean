using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GLMS.Data;
using GLMS.Models;
using GLMS.Factories;
using GLMS.ViewModels;
using GLMS.Services;

namespace GLMS.Controllers
{
    public class ContractsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileStorageService _fileStorage;

        public ContractsController(ApplicationDbContext context, IFileStorageService fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
        }

        // GET: Contracts
        public async Task<IActionResult> Index(ContractFilterViewModel filter)
        {
            IQueryable<Contract> query = _context.Contracts.Include(c => c.Client);

            if (filter.StartDateFrom.HasValue)
            {
                query = query.Where(c => c.StartDate >= filter.StartDateFrom.Value);
            }

            if (filter.StartDateTo.HasValue)
            {
                query = query.Where(c => c.StartDate <= filter.StartDateTo.Value);

            }

            if (filter.Status.HasValue)
            {
                query = query.Where(c => c.Status == filter.Status.Value);
            }

            query = query.OrderByDescending(c => c.StartDate);
            filter.Results = await query.ToListAsync();

            return View(filter);
        }
        // GET: Contracts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // GET: Contracts/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name");
            ViewData["ContractType"] = new SelectList(ContractFactoryResolver.AvailableTypes);
            return View(new ContractCreateViewModel());
        }

        // POST: Contracts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContractCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    IContractFactory factory = ContractFactoryResolver.GetFactory(viewModel.ContractType);
                    IContract contractAbstraction = factory.CreateContract(
                        viewModel.StartDate,
                        viewModel.EndDate,
                        viewModel.ServiceLevel);

                    Contract contractEntity = contractAbstraction.ToEntity(viewModel.ClientId);

                    //PDF Upload using the file service
                    string? fileName = null;
                    string? filePath = null;
                    try
                    {
                        (fileName, filePath) = await _fileStorage.SaveAsync(viewModel.SignedAgreement, "contracts");
                    }
                    catch (FileValidationException ex)
                    {
                        ModelState.AddModelError(nameof(viewModel.SignedAgreement), ex.Message);
                        ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name", viewModel.ClientId);
                        ViewData["ContractType"] = new SelectList(ContractFactoryResolver.AvailableTypes, viewModel.ContractType);
                        return View(viewModel);
                    }

                    contractEntity.SignedAgreementFileName = fileName;
                    contractEntity.SignedAgreementFilePath = filePath;

                    _context.Contracts.Add(contractEntity);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError(nameof(viewModel.ContractType), ex.Message);
                }
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name", viewModel.ClientId);
            ViewData["ContractType"] = new SelectList(ContractFactoryResolver.AvailableTypes, viewModel.ContractType);
            return View(viewModel);
        }

        // GET: Contracts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "ContactDetails", contract.ClientId);
            return View(contract);
        }

        // POST: Contracts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClientId,StartDate,EndDate,Status,ServiceLevel,SignedAgreementFileName,SignedAgreementFilePath")] Contract contract)
        {
            if (id != contract.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contract);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractExists(contract.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "ContactDetails", contract.ClientId);
            return View(contract);
        }

        // GET: Contracts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // POST: Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract != null)
            {
                _context.Contracts.Remove(contract);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContractExists(int id)
        {
            return _context.Contracts.Any(e => e.Id == id);
        }
    }
}
