using CMS.Common;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using CMS.Domain.Storage.Services;
using CMS.Web.Helpers;
using CMS.Web.Logger;
using CMS.Web.Models;
using CMS.Web.ViewModels;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;

namespace CMS.Web.Controllers
{
    [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
    public class BoardController : BaseController
    {
        readonly IBoardService _boardService;
        readonly ILogger _logger;
        readonly IRepository _repository;
        readonly IEmailService _emailService;
        readonly IAspNetRoles _aspNetRolesService;
        readonly IBranchAdminService _branchAdminService;
        readonly IClientAdminService _clientAdminService;


        public BoardController(IClientAdminService clientAdminService, IBoardService boardService, ILogger logger, IRepository repository, IEmailService emailService, IAspNetRoles aspNetRolesService, IBranchAdminService branchAdminService)
        {
            _boardService = boardService;
            _logger = logger;
            _repository = repository;
            _emailService = emailService;
            _aspNetRolesService = aspNetRolesService;
            _branchAdminService = branchAdminService;
            _clientAdminService = clientAdminService;
        }

        public ActionResult Index()
        {
            //var branches = _branchService.GetAllBranches().ToList();
            //var viewModelList = AutoMapper.Mapper.Map<List<BranchProjection>, BranchViewModel[]>(branches);
            //return View(viewModelList);
            //return View();

            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            var projection = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;

            /*ViewBag.ClassList = (from c in _clientAdminService.GetClients()
                                 select new SelectListItem
                                 {
                                     Value = c.ClientId.ToString(),
                                     Text = c.Name
                                 }).ToList();*/

            if (roles == "Admin")
            {
                ViewBag.userId = 0;
            }
            else
            {
                ViewBag.userId = projection.ClientId;
            }
            return View();

        }

        public ActionResult Create(int? ClientId)
        {
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);

            if (roles == "Admin")
            {
                var clientList = (from b in _clientAdminService.GetClients()
                                  select new SelectListItem
                                  {
                                      Value = b.ClientId.ToString(),
                                      Text = b.Name
                                  }).ToList();

                ViewBag.ClientId = null;

                return View(new BranchViewModel
                {
                    Clients = clientList,
                    CurrentUserRole = roles
                });
            }
            else if (roles == "Client")
            {
                var projection = _clientAdminService.GetClientAdminById(roleUserId);

                ViewBag.ClientId = projection.ClientId;
                ViewBag.CurrentUserRole = roles;
                return View(new BoardViewModel
                {
                    CurrentUserRole = roles,
                    ClientId = projection.ClientId,
                    ClientName = projection.ClientName
                });
            }

            return View();
        }
   

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BoardViewModel viewModel)
        {

            var roles = viewModel.CurrentUserRole;
            var clientId = viewModel.ClientId;
            var clientName = viewModel.ClientName;
            if (ModelState.IsValid)
            {
                var board = new Board
                { 
                    Name = viewModel.Name,
                    ClientId = viewModel.ClientId,
                    // UserId = viewModel.UserId


                };

                var result = _boardService.Save(board);
                if (result.Success)
                {

                    Success(result.Results.FirstOrDefault().Message);
                    ModelState.Clear();
                }
                else
                {
                    _logger.Warn(result.Results.FirstOrDefault().Message);
                    Warning(result.Results.FirstOrDefault().Message, true);
                    ReturnViewModel(roles, viewModel, clientId, clientName);
                }
            }
            ReturnViewModel(roles, viewModel, clientId, clientName);
            viewModel = new BoardViewModel();

            return View(viewModel);
        }
        public void ReturnViewModel(string roles, BoardViewModel viewModel, int clientId, string clientName)
        {
            if (roles == "Admin")
            {
                var clientList = (from b in _clientAdminService.GetClients()
                                  select new SelectListItem
                                  {
                                      Value = b.ClientId.ToString(),
                                      Text = b.Name
                                  }).ToList();
                viewModel.Clients = clientList;
                ViewBag.ClientId = null;
            }
            else if (roles == "Client")
            {
                viewModel.ClientId = clientId;
                viewModel.ClientName = clientName;
            }

            viewModel.CurrentUserRole = roles;
        }

        public ActionResult Edit(int id)
        {
            var projection = _boardService.GetBoardById(id);
            if (projection == null)
            {
                _logger.Warn(string.Format("Board does not Exists {0}.", id));
                Warning("Board does not Exists.");
                return RedirectToAction("Index");
            }
            var viewModel = AutoMapper.Mapper.Map<BoardProjection, BoardViewModel>(projection);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BoardViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var board = _repository.Project<Board, bool>(boards => (from b in boards where b.BoardId == viewModel.BoardId select b).Any());
                if (!board)
                {
                    _logger.Warn(string.Format("Board not exists '{0}'.", viewModel.Name));
                    Danger(string.Format("Board not exists '{0}'.", viewModel.Name));
                }
                var result = _boardService.Update(new Board { BoardId = viewModel.BoardId, Name = viewModel.Name });
                if (result.Success)
                {
                    var roleUserId = User.Identity.GetUserId();
                    var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
                    var bodySubject = "Web portal changes - Board update";
                    var message = "Board Updated Successfully";
                    SendMailToAdmin(roles, roleUserId, message, viewModel.Name, bodySubject);
                    Success(result.Results.FirstOrDefault().Message);
                    ModelState.Clear();
                    return RedirectToAction("Index");
                }
                else
                {
                    _logger.Warn(result.Results.FirstOrDefault().Message);
                    Warning(result.Results.FirstOrDefault().Message, true);
                }
            }
            return View(viewModel);
        }

        public ActionResult Delete(int id)
        {
            var projection = _boardService.GetBoardById(id);
            if (projection == null)
            {
                _logger.Warn(string.Format("Board does not Exists {0}.", id));
                Warning("Board does not Exists.");
                return RedirectToAction("Index");
            }
            var viewModel = AutoMapper.Mapper.Map<BoardProjection, BoardViewModel>(projection);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(BoardViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = _boardService.Delete(viewModel.BoardId);
                if (result.Success)
                {
                    var roleUserId = User.Identity.GetUserId();
                    var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
                    var message = "Board Deleted Successfully";
                    var bodySubject = "Web portal changes - Board Delete";
                    SendMailToAdmin(roles, roleUserId, message, viewModel.Name, bodySubject);
                    Success(result.Results.FirstOrDefault().Message);
                    ModelState.Clear();
                }
                else
                {
                    _logger.Warn(result.Results.FirstOrDefault().Message);
                    Warning(result.Results.FirstOrDefault().Message, true);
                }
            }
            return RedirectToAction("Index");
        }

        public void SendMailToAdmin(string roles, string roleUserId, string message, string Name, string bodySubject)
        {
            if (roles == "BranchAdmin")
            {
                var branchAdmin = _branchAdminService.GetBranchAdminById(roleUserId);
                var branchName = branchAdmin.BranchName;
                var branchAdminEmail = branchAdmin.Email;
                string body = string.Empty;
                using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/CommonMailDesign.html")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{BranchName}", branchName);
                body = body.Replace("{ModuleName}", "Board Name : " + Name + "<br/>" + message);
                body = body.Replace("{BranchAdminEmail}", "( " + branchAdminEmail + " )");

                var emailMessage = new MailModel
                {
                    Body = body,
                    Subject = bodySubject,
                    IsBranchAdmin = true
                };
                _emailService.Send(emailMessage);
            }
        }
    }
}