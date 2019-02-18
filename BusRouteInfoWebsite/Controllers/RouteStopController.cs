using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusRouteInfoWebsite.Models;
using Microsoft.AspNetCore.Http;

namespace BusRouteInfoWebsite.Controllers
{
    public class RouteStopController : Controller
    {
        private readonly BusServiceContext _context;

        public RouteStopController(BusServiceContext context)
        {
            _context = context;
        }

        // GET: RouteStop
        public async Task<IActionResult> Index(int? busRouteCode, string busRouteName)
        {
 
            if (busRouteCode!=0 && busRouteCode!=null)
            {
                HttpContext.Session.SetString(nameof(busRouteCode), busRouteCode.ToString());
                if (string.IsNullOrEmpty(busRouteName))
                {
                    HttpContext.Session.SetString(nameof(busRouteName), _context.BusRoute.SingleOrDefault(r => r.BusRouteCode == busRouteCode.ToString()).RouteName);
                }
                else
                {
                    HttpContext.Session.SetString(nameof(busRouteName), busRouteName);
                }
            }
            else if (HttpContext.Session.GetString(nameof(busRouteCode))!=null)
            {
                busRouteCode = Convert.ToInt32(HttpContext.Session.GetString(nameof(busRouteCode)));
                HttpContext.Session.SetString(nameof(busRouteName), _context.BusRoute.SingleOrDefault(r => r.BusRouteCode == busRouteCode.ToString()).RouteName);
            }
            else
            {
                TempData["message"] = "Please select a valid bus route";
                return RedirectToAction("Index", "BusRoute");
            }

            ViewData["stop"] = HttpContext.Session.GetString(nameof(busRouteName));
            ViewData["route"] = HttpContext.Session.GetString(nameof(busRouteCode));
            var busServiceContext = _context.RouteStop.Include(r => r.BusRouteCodeNavigation).Include(r => r.BusStopNumberNavigation).Where(r=>r.BusRouteCode==busRouteCode.ToString()).OrderBy(r=>r.OffsetMinutes);

            return View(await busServiceContext.ToListAsync());
        }

        // GET: RouteStop/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var routeStop = await _context.RouteStop
                .Include(r => r.BusRouteCodeNavigation)
                .Include(r => r.BusStopNumberNavigation)
                .FirstOrDefaultAsync(m => m.RouteStopId == id);
            if (routeStop == null)
            {
                return NotFound();
            }

            return View(routeStop);
        }

        // GET: RouteStop/Create
        public IActionResult Create()
        {
            //**********************************************Special Atention******************************************************************
            //****************if you want the record to be insert into correct bus route, you need to speciify the code you have selected*****
            //********************************************************************************************************************************
            ViewData["BusRouteCode"] = new SelectList(_context.BusRoute.Where(r=>r.BusRouteCode== HttpContext.Session.GetString("busRouteCode")), "BusRouteCode", "BusRouteCode");
            ViewData["BusStopNumber"] = new SelectList(_context.BusStop.OrderBy(b=>b.Location), "BusStopNumber", "Location");
            //Alternative way
            //ViewData["Location"] = new SelectList(_context.BusStop.OrderBy(l=>l.Location), "Location", "Location");
            ViewData["OffsetMinutes"] = new SelectList(_context.RouteStop, "OffsetMinutes", "OffsetMinutes");
            return View();
        }

        // POST: RouteStop/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RouteStopId,BusRouteCode,BusStopNumber,OffsetMinutes")] RouteStop routeStop)
        {
            RouteStop newroute = routeStop;
            //************************************************************IMPORTANT*************************************************************
            //************************************************************IMPORTANT*************************************************************
            //************************************************************IMPORTANT*************************************************************            
            //You have to control the route number and  and stop number in the database to ensure you select specific one for that typical route
            //************************************************************IMPORTANT*************************************************************
            //************************************************************IMPORTANT*************************************************************
            //************************************************************IMPORTANT*************************************************************

            if (_context.RouteStop.Where(r => r.BusRouteCode == newroute.BusRouteCode).Any(r => r.BusStopNumber == newroute.BusStopNumber) || (_context.RouteStop.Where(r => r.BusStopNumber == newroute.BusStopNumber).Any(r => r.OffsetMinutes == newroute.OffsetMinutes)))
            {
                if (_context.RouteStop.Where(r=>r.BusRouteCode == newroute.BusRouteCode).Any(r => r.BusStopNumber == newroute.BusStopNumber))

                    TempData["Routemessage"] = "This route already exists, please select new one ";
                if (_context.RouteStop.Where(r => r.BusStopNumber == newroute.BusStopNumber && r.BusRouteCode==newroute.BusRouteCode).Any(r => r.OffsetMinutes == newroute.OffsetMinutes))
                    TempData["Stopmessage"] = "This offset already exists, please select new one";
                return RedirectToAction("Create", "RouteStop");


            }
            //This allows user to specify the offsetminutes for the typical busstopnumber

            //if(_context.RouteStop.Where(r=>r.BusStopNumber==newroute.BusStopNumber).Any(r => r.OffsetMinutes==newroute.OffsetMinutes))
            //{
            //    TempData["message"] = "This offset already exists, please select new one";
            //    return RedirectToAction("Create", "RouteStop");
            //}

            if (ModelState.IsValid)
            {
                _context.Add(routeStop);
                TempData["message"] = "Stop sucessfully created";
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //**********************************************Special Atention******************************************************************
            //****************if you want the record to be insert into correct bus route, you need to speciify the code you have selected*****
            //********************************************************************************************************************************
            ViewData["BusRouteCode"] = new SelectList(_context.BusRoute.Where(r => r.BusRouteCode == HttpContext.Session.GetString("busRouteCode")), "BusRouteCode", "BusRouteCode", routeStop.BusRouteCode);
            //ViewData["BusStopNumber"] = new SelectList(_context.BusStop, "BusStopNumber", "BusStopNumber", routeStop.BusStopNumber);
            ViewData["BusStopNumber"] = new SelectList(_context.BusStop.OrderBy(b => b.Location), "BusStopNumber", "Location", routeStop.BusStopNumber);

            ViewData["OffsetMinutes"] = new SelectList(_context.RouteStop, "OffsetMinutes", "OffsetMinutes", routeStop.OffsetMinutes);
            return View(routeStop);
        }

        // GET: RouteStop/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var routeStop = await _context.RouteStop.FindAsync(id);
            if (routeStop == null)
            {
                return NotFound();
            }
            ViewData["BusRouteCode"] = new SelectList(_context.BusRoute, "BusRouteCode", "BusRouteCode", routeStop.BusRouteCode);
            ViewData["BusStopNumber"] = new SelectList(_context.BusStop, "BusStopNumber", "BusStopNumber", routeStop.BusStopNumber);
            return View(routeStop);
        }

        // POST: RouteStop/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RouteStopId,BusRouteCode,BusStopNumber,OffsetMinutes")] RouteStop routeStop)
        {
            if (id != routeStop.RouteStopId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(routeStop);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RouteStopExists(routeStop.RouteStopId))
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
            ViewData["BusRouteCode"] = new SelectList(_context.BusRoute, "BusRouteCode", "BusRouteCode", routeStop.BusRouteCode);
            ViewData["BusStopNumber"] = new SelectList(_context.BusStop, "BusStopNumber", "BusStopNumber", routeStop.BusStopNumber);
            return View(routeStop);
        }

        // GET: RouteStop/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var routeStop = await _context.RouteStop
                .Include(r => r.BusRouteCodeNavigation)
                .Include(r => r.BusStopNumberNavigation)
                .FirstOrDefaultAsync(m => m.RouteStopId == id);
            if (routeStop == null)
            {
                return NotFound();
            }

            return View(routeStop);
        }

        // POST: RouteStop/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var routeStop = await _context.RouteStop.FindAsync(id);
            _context.RouteStop.Remove(routeStop);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RouteStopExists(int id)
        {
            return _context.RouteStop.Any(e => e.RouteStopId == id);
        }
    }
}
